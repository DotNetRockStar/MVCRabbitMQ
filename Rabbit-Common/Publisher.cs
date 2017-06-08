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
    /// <summary>
    /// Class that performs basic message publishing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Publisher<T> : IDisposable, IQueueCreator
    {
        /// <summary>
        /// The channel created for the instance of this class.
        /// </summary>
        public IModel Channel { get; private set; }
        public IConnection Connection { get; private set; }
        public string QueueName { get; private set; }

        /// <summary>
        /// The exchange set for the instance of this class.  Messages being listened for will listen on the exchange specified.
        /// *** WARNING: If the exchange is set for this class and a queue already exists for the message that this class is listeneing for and that exchange
        /// is different than the one specified, then there could be undesired side effects.
        /// </summary>
        protected Exchange Exchange { get; private set; }
        /// <summary>
        /// The RabbitMQ queue settings created or specified for this instance of this class.  These are the queue settings used to create the appropriate queue
        /// for listening for a published message.
        /// </summary>
        protected IQueueSettings QueueSettings { get; private set; }
        /// <summary>
        /// The RabbitMQ server settings created or specified for this class.  These are the settings used to connect to the RabbitMQ server.
        /// </summary>
        protected IRabbitServerSettings ServerSettings { get; private set; }

        /// <summary>
        /// Create instance of publisher class to publish messages.
        /// </summary>
        public Publisher()
        {
            ConstructorSetup(new Settings() { QueueName = typeof(T).FullName, RoutingKey = typeof(T).FullName }, new ConfigRabbitServerSettings());
        }
        /// <summary>
        /// Create instance of publisher class to publish messages.
        /// </summary>
        /// <param name="serverSettings">The RabbitMQ server settings that should be used for connecting to the RabbitMQ server.</param>
        public Publisher(IRabbitServerSettings serverSettings)
        {
            ConstructorSetup(new Settings() { QueueName = typeof(T).FullName, RoutingKey = typeof(T).FullName }, serverSettings);
        }

        /// <summary>
        /// Create instance of publisher class to publish messages.
        /// </summary>
        /// <param name="exchange">The exchange for which this listener should listen for messages on.</param>
        public Publisher(Exchange exchange)
        {
            ConstructorSetup(new Settings() { QueueName = typeof(T).FullName, RoutingKey = typeof(T).FullName }, new ConfigRabbitServerSettings(), exchange);
        }
        /// <summary>
        /// Create instance of publisher class to publish messages.
        /// </summary>
        /// <param name="exchange">The exchange for which this listener should listen for messages on.</param>
        /// <param name="serverSettings">The RabbitMQ server settings that should be used for connecting to the RabbitMQ server.</param>
        public Publisher(Exchange exchange, IRabbitServerSettings serverSettings)
        {
            ConstructorSetup(new Settings() { QueueName = typeof(T).FullName, RoutingKey = typeof(T).FullName }, serverSettings, exchange);
        }

        /// <summary>
        /// Create instance of publisher class to publish messages.
        /// </summary>
        /// <param name="queueSettings">The RabbitMQ queue settings that should be applied to the queue that is created for the message that this listener is listenining for.</param>
        public Publisher(IQueueSettings queueSettings)
        {
            ConstructorSetup(queueSettings, new ConfigRabbitServerSettings());
        }
        /// <summary>
        /// Create instance of publisher class to publish messages.
        /// </summary>
        /// <param name="queueSettings">The RabbitMQ queue settings that should be applied to the queue that is created for the message that this listener is listenining for.</param>
        /// <param name="serverSettings">The RabbitMQ server settings that should be used for connecting to the RabbitMQ server.</param>
        public Publisher(IQueueSettings queueSettings, IRabbitServerSettings serverSettings)
        {
            ConstructorSetup(queueSettings, serverSettings);
        }

        /// <summary>
        /// Create instance of publisher class to publish messages.
        /// </summary>
        /// <param name="queueSettings">The RabbitMQ queue settings that should be applied to the queue that is created for the message that this listener is listenining for.</param>
        /// <param name="exchange">The exchange for which this listener should listen for messages on.</param>
        public Publisher(IQueueSettings queueSettings, Exchange exchange)
        {
            ConstructorSetup(queueSettings, new ConfigRabbitServerSettings(), exchange);
        }
        /// <summary>
        /// Create instance of publisher class to publish messages.
        /// </summary>
        /// <param name="queueSettings">The RabbitMQ queue settings that should be applied to the queue that is created for the message that this listener is listenining for.</param>
        /// <param name="exchange">The exchange for which this listener should listen for messages on.</param>
        /// <param name="serverSettings">The RabbitMQ server settings that should be used for connecting to the RabbitMQ server.</param>
        public Publisher(IQueueSettings queueSettings, Exchange exchange, IRabbitServerSettings serverSettings)
        {
            ConstructorSetup(queueSettings, serverSettings, exchange);
        }

        private void ConstructorSetup(IQueueSettings settings, IRabbitServerSettings serverSettings, Exchange exchange = null)
        {
            if (settings == null)
                throw new ArgumentNullException("queueSettings");
            this.QueueSettings = settings;
            this.ServerSettings = ServerSettings;

            this.Exchange = exchange;
            string queue = settings.QueueName;
            if (string.IsNullOrWhiteSpace(queue))
                queue = typeof(T).FullName;
            settings.QueueName = queue;
            this.QueueName = queue;

            Channel = CreateChannel(Exchange, settings, serverSettings);
            this.CreateQueue(this.Channel, exchange, settings, serverSettings);
        }

        /// <summary>
        /// Method used to set up connection, queue, and queue binding.
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queueName"></param>
        /// <param name="routingKey"></param>
        protected virtual IModel CreateChannel(Exchange exchange, IQueueSettings settings, IRabbitServerSettings serverSettings)
        {
            IConnection connection = null;
            IModel channel = null;

            if (exchange != null)
            {
                connection = exchange.Connection;
                channel = exchange.Channel;
            }
            else
            {
                var connectionFactory = new ExtendedConnectionFactory(serverSettings);
                connection = connectionFactory.CreateConnection();
                channel = connection.CreateModel();
                connection.AutoClose = true;
            }

            this.Connection = connection;
            return channel;
        }

        protected virtual QueueDeclareOk CreateQueue(IModel channel, Exchange exchange, IQueueSettings settings, IRabbitServerSettings serverSettings)
        {
            var results = channel.QueueDeclare(settings.QueueName, settings.IsDurable, settings.IsExclusive, settings.AutoDelete, settings.Arguments);

            QueueManager.AddCreatedQueue(channel, settings.QueueName);

            if (exchange != null)
            {
                var rules = RegistrationUtility.ResolveRules(null);
                if (!rules.IsQueueApproved(serverSettings.Host, settings.QueueName))
                {
                    throw new Exception("Unable to create queue " + settings.QueueName + " because the queue has not been authorized for creation by the administrative team.");
                }

                if (!rules.IsExchangeApproved(serverSettings.Host, exchange.Name, exchange.Type))
                    throw new Exception("Unable to create queue exchange " + Exchange.Name + " because the exchange has not been authorized for creation by the administrative team.");

                channel.QueueBind(settings.QueueName, exchange.Name, settings.RoutingKey == null ? "" : settings.RoutingKey);
            }

            return results;
        }

        public virtual void Publish(T item, string routingKey)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
            string exchangeName = string.Empty;
            if (Exchange != null)
                exchangeName = Exchange.Name;

            Channel.BasicPublish(exchangeName, routingKey == null ? "" : routingKey, null, bytes);
        }
        public virtual void Publish(T item)
        {
            this.Publish(item, typeof(T).FullName);
        }

        public void Dispose()
        {
        }
    }
}
