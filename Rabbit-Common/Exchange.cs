using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    public class Exchange : IDisposable
    {
        private IConnection _connection = null;
        internal IConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        private IModel _channel = null;
        internal IModel Channel
        {
            get
            {
                return _channel;
            }
        }
        public string Name { get; private set; }
        public ExchangeType Type { get; private set; }

        public bool PurgeAfterDispose { get; set; }

        public Exchange(string name, ExchangeType type)
        {
            ConfigSectionUtility.ValidateConfigSection();

            this.Name = name;
            this.Type = type;
            this.PurgeAfterDispose = true;

            var connectionFactory = new ExtendedConnectionFactory();
            _connection = connectionFactory.CreateConnection();

            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(this.Name, this.Type.ToString().ToLower(), true);
            ExchangeManager.AddCreatedExchange(this);
        }

        public void Dispose()
        {
            if (PurgeAfterDispose)
                Delete();

            if (_channel.IsOpen)
                _channel.Close();
        }

        public void Delete()
        {
            if (_channel != null)
                _channel.ExchangeDelete(this.Name);
        }
    }

    public enum ExchangeType
    {
        /// <summary>
        /// all messages that are published go to every client.
        /// </summary>
        Fanout,
        /// <summary>
        /// the client can define a filter and then only message that match the filter will get delivered to the client.
        /// </summary>
        Direct,
        /// <summary>
        /// much like direct except you can use wildcards in your topics to fine tune exactly what is delivered to each client.
        /// https://www.rabbitmq.com/tutorials/tutorial-five-python.html
        /// </summary>
        Topic
    }
}
