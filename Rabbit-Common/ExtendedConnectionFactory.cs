using Jerrod.RabbitCommon;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    /// <summary>
    /// Class that extends the ConnectionFactory from the RabbitMQ assembly and takes IRabbitServerSettings in the constructor which
    /// are used to create connections.
    /// </summary>
    public class ExtendedConnectionFactory : ConnectionFactory
    {
        private readonly IRabbitServerSettings _settings;

        public ExtendedConnectionFactory(IRabbitServerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            _settings = settings;

            if (_settings.Port != 0)
                this.Port = _settings.Port;
            if (!string.IsNullOrWhiteSpace(_settings.Host))
                this.HostName = _settings.Host;
            if (!string.IsNullOrWhiteSpace(_settings.Username))
                this.UserName = _settings.Username;
            if (!string.IsNullOrWhiteSpace(_settings.Password))
                this.Password = _settings.Password;
        }
        public ExtendedConnectionFactory()
            : this(new ConfigRabbitServerSettings())
        { }

        public override IConnection CreateConnection()
        {
            return base.CreateConnection();
        }
    }
}
