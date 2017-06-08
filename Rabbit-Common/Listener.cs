using Newtonsoft.Json;
using Jerrod.RabbitCommon;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jerrod.RabbitCommon.Framework;

namespace Jerrod.RabbitCommon
{
    public delegate void ListenForHandler<T>(ListenerEventArgs<T> args);
    public delegate void ExceptionHandler(Exception ex);

    /// <summary>
    /// Class used for basic listening for messages.  This class will only listen for messages of type T.  If more than one type
    /// should be listened to then multiple instances of this class should be created.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Listener<T> : IDisposable, IQueueCreator
    {
        public string QueueName { get; private set; }
        public IConnection Connection { get; private set; }

        /// <summary>
        /// The thread used for listening for messages.
        /// </summary>
        private Thread thread;
        /// <summary>
        /// The channel created for the instance of this class.
        /// </summary>
        public IModel Channel { get; private set; }

        /// <summary>
        /// The event that is invoked when message of type T is detected.
        /// </summary>
        public event ListenForHandler<T> Listen;
        /// <summary>
        /// The event that is invoked when an exception occurs when attempting to process a message detected.
        /// </summary>
        public event ExceptionHandler Exception;

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
        /// Create an instance of the listener class.
        /// </summary>
        public Listener()
        {
            ConstructorSetup(new Settings()
            {
                QueueName = typeof(T).FullName,
                RoutingKey = typeof(T).FullName
            }, new ConfigRabbitServerSettings());
        }
        /// <summary>
        /// Create an instance of the listener class.
        /// </summary>
        /// <param name="serverSettings">The RabbitMQ server settings that should be used for connecting to the RabbitMQ server.</param>
        public Listener(IRabbitServerSettings serverSettings)
        {
            ConstructorSetup(new Settings() { QueueName = typeof(T).FullName, RoutingKey = typeof(T).FullName }, serverSettings: serverSettings);
        }

        /// <summary>
        /// Create an instance of the listener class.
        /// </summary>
        /// <param name="settings">The RabbitMQ queue settings that should be applied to the queue that is created for the message that this listener is listenining for.</param>
        public Listener(IQueueSettings settings)
        {
            ConstructorSetup(settings, new ConfigRabbitServerSettings());
        }
        /// <summary>
        /// Create an instance of the listener class.
        /// </summary>
        /// <param name="settings">The RabbitMQ queue settings that should be applied to the queue that is created for the message that this listener is listenining for.</param>
        /// <param name="serverSettings">The RabbitMQ server settings that should be used for connecting to the RabbitMQ server.</param>
        public Listener(IQueueSettings settings, IRabbitServerSettings serverSettings)
        {
            ConstructorSetup(settings, serverSettings: serverSettings);
        }

        /// <summary>
        /// Create an instance of the listener class.
        /// </summary>
        /// <param name="exchange">The exchange for which this listener should listen for messages on.</param>
        public Listener(Exchange exchange)
        {
            ConstructorSetup(new Settings() { QueueName = typeof(T).FullName, RoutingKey = typeof(T).FullName }, new ConfigRabbitServerSettings(), exchange);
        }
        /// <summary>
        /// Create an instance of the listener class.
        /// </summary>
        /// <param name="exchange">The exchange for which this listener should listen for messages on.</param>
        /// <param name="serverSettings">The RabbitMQ server settings that should be used for connecting to the RabbitMQ server.</param>
        public Listener(Exchange exchange, IRabbitServerSettings serverSettings)
        {
            ConstructorSetup(new Settings() { QueueName = typeof(T).FullName, RoutingKey = typeof(T).FullName }, serverSettings, exchange);
        }

        /// <summary>
        /// Create an instance of the listener class.
        /// </summary>
        /// <param name="settings">The RabbitMQ queue settings that should be applied to the queue that is created for the message that this listener is listenining for.</param>
        /// <param name="exchange">The exchange for which this listener should listen for messages on.</param>
        public Listener(IQueueSettings settings, Exchange exchange)
        {
            ConstructorSetup(settings, new ConfigRabbitServerSettings(), exchange);
        }
        /// <summary>
        /// Create an instance of the listener class.
        /// </summary>
        /// <param name="settings">The RabbitMQ queue settings that should be applied to the queue that is created for the message that this listener is listenining for.</param>
        /// <param name="exchange">The exchange for which this listener should listen for messages on.</param>
        /// <param name="serverSettings">The RabbitMQ server settings that should be used for connecting to the RabbitMQ server.</param>
        public Listener(IQueueSettings settings, Exchange exchange, IRabbitServerSettings serverSettings)
        {
            ConstructorSetup(settings, serverSettings, exchange);
        }

        private void ConstructorSetup(IQueueSettings settings, IRabbitServerSettings serverSettings, Exchange exchange = null)
        {
            ConfigSectionUtility.ValidateConfigSection();
            this.ServerSettings = serverSettings;

            string queue = settings.QueueName;
            if (string.IsNullOrWhiteSpace(queue))
                queue = typeof(T).FullName;

            settings.QueueName = queue;
            this.QueueName = queue;
            Exchange = exchange;

            if (settings.RoutingKey == null)
                settings.RoutingKey = typeof(T).FullName;

            this.QueueSettings = settings;

            var connectionFactory = new ExtendedConnectionFactory();
            var connection = connectionFactory.CreateConnection();

            Channel = CreateChannel(exchange, serverSettings, settings);
            CreateQueue(this.Channel, this.Exchange, serverSettings, settings);

            connection.AutoClose = true; // must do after channel creation

            Listen += Listener_ListenFor;
            Exception += Listener_Exception;

            thread = new Thread(new ThreadStart(ProcessQueue));
            thread.IsBackground = true;
            // this is performed from a bg thread, to ensure the queue is serviced from a single thread
            thread.Start();
        }

        /// <summary>
        /// This method gets called indefinately until the disposal of this class.  This class is intended to listen
        /// for messages being sent.  Default behavior uses BasicGet
        /// </summary>
        /// <param name="channel"></param>
        protected virtual T TryReadMessage(IModel channel, string queueName, Exchange exchange)
        {
            BasicGetResult result = channel.BasicGet(queueName, true);

            if (result != null)
            {
                if (result.Body == null)
                    return default(T);

                string message = Encoding.UTF8.GetString(result.Body);

                if (message != null)
                {
                    T obj = JsonConvert.DeserializeObject<T>(message);
                    return obj;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Method used to set up connection, queue, and queue binding.
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queueName"></param>
        /// <param name="routingKey"></param>
        protected virtual IModel CreateChannel(Exchange exchange, IRabbitServerSettings serverSettings, IQueueSettings settings)
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

        /// <summary>
        /// Method invoked when the instance of this class is attempting to create the queue required for listening to the message of type T.
        /// </summary>
        /// <param name="channel">The channel that was created and should be used for creation of the queue.</param>
        /// <param name="exchange">The exchange that was specified and should be used for listening for the message on the queue.</param>
        /// <param name="serverSettings">The server settings created or specified that should be used for connecting to the correct RabbitMQ server</param>
        /// <param name="settings">The queue settings created or specified that should be applied to the queue created.</param>
        /// <returns></returns>
        protected virtual QueueDeclareOk CreateQueue(IModel channel, Exchange exchange, IRabbitServerSettings serverSettings, IQueueSettings settings)
        {
            var rules = RegistrationUtility.ResolveRules(null);
            if (!rules.IsQueueApproved(serverSettings.Host, settings.QueueName))
            {
                throw new Exception("Unable to create queue " + settings.QueueName + " because the queue has not been authorized for creation by the administrative team.");
            }

            var queue = channel.QueueDeclare(settings.QueueName, settings.IsDurable, settings.IsExclusive, settings.AutoDelete, settings.Arguments);

            QueueManager.AddCreatedQueue(channel, settings.QueueName);
            
            if (exchange != null)
            {
                if (!rules.IsExchangeApproved(ServerSettings.Host, this.Exchange.Name, this.Exchange.Type))
                    throw new Exception("Unable to create queue exchange " + Exchange.Name + " because the exchange has not been authorized for creation by the administrative team.");
                
                channel.QueueBind(settings.QueueName, exchange.Name, settings.RoutingKey == null ? "" : settings.RoutingKey);
            }

            return queue;
        }

        /// <summary>
        /// Attempt to abort the thread being used to listen for messages.  If PurgeQueueOnDispose is set to true then delete the queue created.
        /// </summary>
        public void Dispose()
        {
            thread.Abort();
        }

        private void Listener_Exception(Exception ex)
        {
        }
        private void Listener_ListenFor(ListenerEventArgs<T> args)
        {
        }

        private void ProcessQueue()
        {
            bool _shouldStop = false;
            while (!_shouldStop)
            {
                try
                {
                    T item = TryReadMessage(Channel, this.QueueSettings.QueueName, this.Exchange);

                    if (item != null)
                    {
                        try
                        {
                            Listen(new ListenerEventArgs<T>() { Listener = this, Item = item });
                        }
                        catch (Exception iEx)
                        {
                            Exception(iEx);
                        }
                    }
                }
                catch
                {
                    _shouldStop = true;
                }
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}