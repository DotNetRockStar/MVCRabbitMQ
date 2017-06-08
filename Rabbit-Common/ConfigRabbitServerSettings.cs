using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    /// <summary>
    /// Rabbit settings class that looks in the ConfigSection of the app.config/web.config file for values required to connect to a RabbitMQ server.
    /// </summary>
    public class ConfigRabbitServerSettings : IRabbitServerSettings
    {
        public ConfigRabbitServerSettings()
        {
            if (!ConfigSectionUtility.IsConfigSectionValid)
                throw new ApplicationException("web.config or app.config must contain \"rabbit\" config section to use ConfigRabbitServerSettings.");
        }
        public string Host
        {
            get { return ConfigSectionUtility.Host; }
        }

        public int Port
        {
            get { return ConfigSectionUtility.Port; }
        }

        public string Username
        {
            get { return ConfigSectionUtility.Username; }
        }

        public string Password
        {
            get { return ConfigSectionUtility.Password; }
        }
    }
}
