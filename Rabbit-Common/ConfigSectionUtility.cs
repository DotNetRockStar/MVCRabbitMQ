using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    /// <summary>
    /// A helper utility class that will read values set in the app.config/web.config if it exists.
    /// </summary>
    public static class ConfigSectionUtility
    {
        /// <summary>
        /// Return the instance of the RabbitConfigSection.
        /// </summary>
        public static RabbitConfigSection ConfigSection
        {
            get
            {
                var config = ConfigurationManager.GetSection("rabbit") as RabbitConfigSection;
                if (config == null)
                    return null;

                return config;
            }
        }

        /// <summary>
        /// Return the host property out of the config section.
        /// </summary>
        public static string Host
        {
            get
            {
                var config = ConfigSection;
                if (config == null)
                    return null;
                return config.Host;
            }
        }

        /// <summary>
        /// Return the username property out of the config section.
        /// </summary>
        public static string Username
        {
            get
            {
                var config = ConfigSection;
                if (config == null)
                    return null;
                return config.Username;
            }
        }

        /// <summary>
        /// Return the password property out of the config section.
        /// </summary>
        public static string Password
        {
            get
            {
                var config = ConfigSection;
                if (config == null)
                    return null;
                return config.Password;
            }
        }

        /// <summary>
        /// Return the port property out of the config section.
        /// </summary>
        public static int Port
        {
            get
            {
                var config = ConfigSection;
                if (config == null)
                    return 0;
                return config.Port;
            }
        }

        /// <summary>
        /// A flag determining if the config section is available and valid.
        /// </summary>
        public static bool IsConfigSectionValid
        {
            get
            {
                return ConfigSection != null;
            }
        }

        /// <summary>
        /// A method that is called where the config section is validated.  If it is not valid an exception will occur.
        /// </summary>
        public static void ValidateConfigSection()
        {
            if (!IsConfigSectionValid)
                throw new ApplicationException("ConfigSection Jerrod.rabbit does not exist or is configured incorrectly.");

            var section = ConfigSection;

            if (!string.IsNullOrWhiteSpace(section.Username) && string.IsNullOrWhiteSpace(section.Password))
                throw new ApplicationException("ConfigSection contains a username but not password.");
            else if (string.IsNullOrWhiteSpace(section.Username) && !string.IsNullOrWhiteSpace(section.Password))
                throw new ApplicationException("ConfigSection contains a password but not a username.");
        }

        /// <summary>
        /// Sql database rules connection string property out of the config section.
        /// </summary>
        public static string RulesConnectionString
        {
            get
            {
                var section = ConfigSection;
                return section.RulesConnectionString;
            }
        }
    }
}
