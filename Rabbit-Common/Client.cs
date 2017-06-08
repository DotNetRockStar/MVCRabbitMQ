using Newtonsoft.Json;
using Jerrod.RabbitCommon;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jerrod.RabbitCommon.Framework;

namespace Jerrod.RabbitCommon
{
    public class Client<T> : Publisher<T>, IDisposable
    {
        private QueueingBasicConsumer _consumer;
        private string CallbackQueueName = null;
        private bool _hasCallbackQueueBeenCreated = false;
        private int _timeoutInSeconds = 30;
        private readonly IRabbitServerSettings _serverSettings;

        /// <summary>
        /// Set the amount of time, in seconds, that a request should wait for a response when this.Call is invoked.
        /// </summary>
        public int TimeoutInSeconds
        {
            get
            {
                return _timeoutInSeconds;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException("TimeoutInSeconds must be greater than 0.");
                _timeoutInSeconds = value;
            }
        }

        /// <summary>
        /// Create instance of client to publish messages.
        /// </summary>
        public Client()
            : base()
        {
            this._serverSettings = new ConfigRabbitServerSettings();
        }

        /// <summary>
        /// Create instance of client to publish messages.
        /// </summary>
        /// <param name="exchange">Exchange that should be used when publishing messages.</param>
        public Client(Exchange exchange)
            : base(exchange)
        {
            this._serverSettings = new ConfigRabbitServerSettings();
        }

        /// <summary>
        /// Create instance of client to publish messages.
        /// </summary>
        /// <param name="exchange">Exchange that should be used when publishing messages.</param>
        /// <param name="serverSettings">Server settings to use when publishing messages.</param>
        public Client(Exchange exchange, IRabbitServerSettings serverSettings)
            : base(exchange)
        {
            if (serverSettings == null)
                throw new ArgumentNullException("serverSettings");

            this._serverSettings = serverSettings;
        }

        private void CreateCallbackQueue()
        {
            if (_hasCallbackQueueBeenCreated)
                return;
            _hasCallbackQueueBeenCreated = true;

            var rules = RegistrationUtility.ResolveRules(null);
            if (!rules.IsQueueApproved(this._serverSettings.Host, this.QueueSettings.QueueName))
            {
                throw new Exception("Unable to create queue " + this.QueueSettings.QueueName + " because the queue has not been authorized for creation by the administrative team.");
            }

            this.CallbackQueueName = this.QueueSettings.QueueName + "_rpc_" + Guid.NewGuid().ToString("N");

            var connectionFactory = new ExtendedConnectionFactory();
            var connection = connectionFactory.CreateConnection();

            var _channel = connection.CreateModel();

            connection.AutoClose = true;
            var queue = _channel.QueueDeclare(queue: CallbackQueueName,
                                    durable: this.QueueSettings.IsDurable,
                                    exclusive: this.QueueSettings.IsExclusive,
                                    autoDelete: this.QueueSettings.AutoDelete,
                                    arguments: this.QueueSettings.Arguments);

            QueueManager.AddCreatedQueue(_channel, CallbackQueueName);
            
            if (this.Exchange != null)
            {
                if (!rules.IsExchangeApproved(ServerSettings.Host, this.Exchange.Name, this.Exchange.Type))
                    throw new Exception("Unable to create queue exchange " + Exchange.Name + " because the exchange has not been authorized for creation by the administrative team.");

                _channel.QueueBind(CallbackQueueName, this.Exchange.Name, this.QueueSettings.RoutingKey == null ? "" : this.QueueSettings.RoutingKey);
            }

            _consumer = new QueueingBasicConsumer(_channel);
            _channel.BasicConsume(queue: CallbackQueueName,
                                  noAck: true,
                                  consumer: _consumer);
        }

        /// <summary>
        /// Publish message and do not wait for result.
        /// </summary>
        /// <param name="item">The message to be published.</param>
        public override void Publish(T item)
        {
            base.Publish(item);
        }
        /// <summary>
        /// Publish message and do not wait for result.
        /// </summary>
        /// <param name="item">The message to be published.</param>
        /// <param name="routingKey">The routing key to be published with the message.</param>
        public override void Publish(T item, string routingKey)
        {
            base.Publish(item, routingKey);
        }

        private byte[] GetResponse(T item)
        {
            var corrId = Guid.NewGuid().ToString();
            var props = this.Channel.CreateBasicProperties();
            props.ReplyTo = CallbackQueueName;
            
            props.CorrelationId = corrId;

            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
            this.Channel.BasicPublish(exchange: this.Exchange == null ? "" : this.Exchange.Name,
                                 routingKey: this.QueueSettings.RoutingKey,
                                 basicProperties: props,
                                 body: bytes);

            byte[] response = null;

            BasicDeliverEventArgs ea = null;
            _consumer.Queue.Dequeue(TimeoutInSeconds * 1000, out ea);
            if (ea == null)
            {
                throw new TimeoutException("Timeout occured while listeneing for response on queue " + this.QueueSettings.RoutingKey);
            }
            else if (ea.BasicProperties.CorrelationId == corrId)
            {
                response = ea.Body;
            }

            return response;
        }

        /// <summary>
        /// Publishes a message and awaits a response.  Once the response is received this method will return.  Even if response contained
        /// a message it will not be returned for this method.
        /// The request will time out after TimeoutInSeconds has been reached.
        /// https://www.rabbitmq.com/tutorials/tutorial-six-python.html
        /// </summary>
        /// <param name="message"></param>
        public void Call(T message)
        {
            CreateCallbackQueue();
            GetResponse(message);
        }

        /// <summary>
        /// Publishes a message and awaits a response.  Once the response is received, this method will attempt to return the response message specified
        /// in the generic parameter specified.  If the response was null or the response was unable to be deserialized, due to incorrect type specified, then
        /// default(typeof(TResponse)) will be returned.
        /// The request will time out after TimeoutInSeconds has been reached.
        /// https://www.rabbitmq.com/tutorials/tutorial-six-python.html
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="message"></param>
        /// <returns>Message sent returned by server handling this message.</returns>
        public TResponse Call<TResponse>(T message)
        {
            CreateCallbackQueue();
            var response = GetResponse(message);
            if (response == null)
                return default(TResponse);

            string text = System.Text.Encoding.UTF8.GetString(response);
            try
            {
                return JsonConvert.DeserializeObject<TResponse>(text);
            }
            catch (Exception ex)
            {
                //throw new ApplicationException("Unable to parse the following string into type " + typeof(TResponse).FullName + ": " + text, ex);
                return default(TResponse);
            }
        }

        /// <summary>
        /// Close all possible connections, channels, and queues created specifically for this instance.
        /// </summary>
        new public void Dispose()
        {
            if (this._hasCallbackQueueBeenCreated)
                this.Channel.DeleteQueue(CallbackQueueName);
            base.Dispose();
        }
    }
}