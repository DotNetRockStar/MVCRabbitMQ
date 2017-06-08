using Newtonsoft.Json;
using Jerrod.RabbitCommon;
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
    public delegate void RPCListenerMethodHandler<T>(T message);

    public class Server<TMessage, TResult> : Listener<TMessage>, IDisposable
    {
        private QueueingBasicConsumer _consumer;
        private readonly IListenerInvoker<TMessage, TResult> _method;

        /// <summary>
        /// Create instance of a server that listens for message and invokes IListenerInvoker specified.
        /// </summary>
        /// <param name="method">The class containing the method that should be invoked when a published message is detected.</param>
        public Server(IListenerInvoker<TMessage, TResult> method)
            : base()
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _method = method;
        }
        /// <summary>
        /// Create instance of a server that listens for message and invokes IListenerInvoker specified.
        /// </summary>
        /// <param name="method">The class containing the method that should be invoked when a published message is detected.</param>
        /// <param name="settings">The queue settings that will be applied to the created queue required for listenining for the message.</param>
        public Server(IListenerInvoker<TMessage, TResult> method, IQueueSettings settings)
            : base(settings)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _method = method;
        }
        /// <summary>
        /// Create instance of a server that listens for message and invokes IListenerInvoker specified.
        /// </summary>
        /// <param name="method">The class containing the method that should be invoked when a published message is detected.</param>
        /// <param name="serverSettings">The server settings to be used for connecting to the RabbitMQ server.</param>
        public Server(IListenerInvoker<TMessage, TResult> method, IRabbitServerSettings serverSettings)
            : base(serverSettings)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _method = method;
        }
        /// <summary>
        /// Create instance of a server that listens for message and invokes IListenerInvoker specified.
        /// </summary>
        /// <param name="method">The class containing the method that should be invoked when a published message is detected.</param>
        /// <param name="serverSettings">The server settings to be used for connecting to the RabbitMQ server.</param>
        /// <param name="settings">The queue settings that will be applied to the created queue required for listenining for the message.</param>
        public Server(IListenerInvoker<TMessage, TResult> method, IRabbitServerSettings serverSettings, IQueueSettings settings)
            : base(settings, serverSettings)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _method = method;
        }

        /// <summary>
        /// Create instance of a server that listens for message and invokes IListenerInvoker specified.
        /// </summary>
        /// <param name="method">The class containing the method that should be invoked when a published message is detected.</param>
        /// <param name="exchange">The exchange to use to listen for messages on the queue.</param>
        public Server(IListenerInvoker<TMessage, TResult> method, Exchange exchange)
            : base(exchange)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _method = method;
        }
        /// <summary>
        /// Create instance of a server that listens for message and invokes IListenerInvoker specified.
        /// </summary>
        /// <param name="method">The class containing the method that should be invoked when a published message is detected.</param>
        /// <param name="exchange">The exchange to use to listen for messages on the queue.</param>
        /// <param name="settings">The queue settings that will be applied to the created queue required for listenining for the message.</param>
        public Server(IListenerInvoker<TMessage, TResult> method, Exchange exchange, IQueueSettings settings)
            : base(settings, exchange)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _method = method;
        }
        /// <summary>
        /// Create instance of a server that listens for message and invokes IListenerInvoker specified.
        /// </summary>
        /// <param name="method">The class containing the method that should be invoked when a published message is detected.</param>
        /// <param name="exchange">The exchange to use to listen for messages on the queue.</param>
        /// <param name="settings">The queue settings that will be applied to the created queue required for listenining for the message.</param>
        /// <param name="serverSettings">The server settings to be used for connecting to the RabbitMQ server.</param>
        public Server(IListenerInvoker<TMessage, TResult> method, Exchange exchange, IQueueSettings settings, IRabbitServerSettings serverSettings)
            : base(settings, exchange, serverSettings)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _method = method;
        }
        /// <summary>
        /// Create instance of a server that listens for message and invokes IListenerInvoker specified.
        /// </summary>
        /// <param name="method">The class containing the method that should be invoked when a published message is detected.</param>
        /// <param name="exchange">The exchange to use to listen for messages on the queue.</param>
        /// <param name="serverSettings">The server settings to be used for connecting to the RabbitMQ server.</param>
        public Server(IListenerInvoker<TMessage, TResult> method, Exchange exchange, IRabbitServerSettings serverSettings)
            : base(exchange, serverSettings)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _method = method;
        }

        protected override QueueDeclareOk CreateQueue(IModel channel, Exchange exchange, IRabbitServerSettings serverSettings, IQueueSettings settings)
        {
            var rules = RegistrationUtility.ResolveRules(null);
            if (!rules.IsQueueApproved(serverSettings.Host, settings.QueueName))
            {
                throw new Exception("Unable to create queue " + settings.QueueName + " because the queue has not been authorized for creation by the administrative team.");
            }

            var queue = channel.QueueDeclare(queue: settings.QueueName,
                                       durable: settings.IsDurable,
                                       exclusive: settings.IsExclusive,
                                       autoDelete: settings.AutoDelete,
                                       arguments: settings.Arguments);

            QueueManager.AddCreatedQueue(channel, settings.QueueName);

            if (exchange != null)
            {
                if (!rules.IsExchangeApproved(ServerSettings.Host, this.Exchange.Name, this.Exchange.Type))
                    throw new Exception("Unable to create queue " + Exchange.Name + " exchange because the exchange has not been authorized for creation by the administrative team.");

                channel.QueueBind(settings.QueueName, exchange.Name, settings.RoutingKey == null ? "" : settings.RoutingKey);
            }

            channel.BasicQos(0, 1, false);

            _consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(queue: settings.QueueName,
                                 noAck: false,
                                 consumer: _consumer);
            return queue;
        }

        protected override TMessage TryReadMessage(IModel channel, string queueName, Exchange exchange)
        {
            BasicDeliverEventArgs ea = null;
            
            ea = (BasicDeliverEventArgs)_consumer.Queue.Dequeue();
          
            var body = ea.Body;
            TMessage obj = default(TMessage);

            var props = ea.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            byte[] response = null;
            string responseExchangeName = exchange == null ? "" : exchange.Name;
            string responseRoutingKey = typeof(TResult).FullName;
            bool supressResponse = false;

            try
            {
                if (body != null)
                {
                    string message = Encoding.UTF8.GetString(body);
                    obj = JsonConvert.DeserializeObject<TMessage>(message);
                }
                var result = ExecuteInjectedMethod(obj);

                if (result != null)
                {
                    if (typeof(TResult).IsAssignableFrom(typeof(RabbitResponse)) || typeof(RabbitResponse).IsAssignableFrom(typeof(TResult)))
                    {
                        RabbitResponse rabbitResponse = (result as RabbitResponse);
                        object resultItem = rabbitResponse.GetResponse();
                        if (!string.IsNullOrWhiteSpace(rabbitResponse.RoutingKey))
                            responseRoutingKey = rabbitResponse.RoutingKey;

                        if (resultItem != null)
                        {
                            if (rabbitResponse._onlyRespondIfRPC && string.IsNullOrWhiteSpace(props.ReplyTo))
                            {
                                supressResponse = true;
                            }
                            else
                            {
                                response = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resultItem));
                                responseRoutingKey = resultItem.GetType().FullName;
                            }
                        }
                        else if (rabbitResponse._onlyRespondIfRPC && string.IsNullOrWhiteSpace(props.ReplyTo))
                            supressResponse = true;
                    }
                    else
                        response = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (!supressResponse && !string.IsNullOrWhiteSpace(props.ReplyTo))
                {
                    channel.BasicPublish(exchange: exchange == null ? "" : exchange.Name,
                                         routingKey: props.ReplyTo,
                                         basicProperties: replyProps,
                                         body: response);
                }
                else if (!supressResponse && response != null) // Should this exist here?  If method returned result but replyto does not exist then put it on a queue?
                {
                    Channel.BasicPublish(responseExchangeName, responseRoutingKey, null, response);
                }

                channel.BasicAck(deliveryTag: ea.DeliveryTag,
                                 multiple: false);
            }

            return obj;
        }

        private TResult ExecuteInjectedMethod(TMessage item)
        {
            return _method.Execute(item);
        }

        new public void Dispose()
        {
            base.Dispose();
            _consumer.Queue.Close();
        }
    }
}
