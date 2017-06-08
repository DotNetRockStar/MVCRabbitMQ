using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    /// <summary>
    /// Config section that allows configuring of the RabbitMQ server connection and the rules.
    /// </summary>
    public class RabbitConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("host", IsRequired=false)]
        public string Host
        {
            get
            {
                return (string)this["host"];
            }
            set
            {
                this["host"] = value;
            }
        }

        [ConfigurationProperty("username", IsRequired = false)]
        public string Username
        {
            get
            {
                return (string)this["username"];
            }
            set
            {
                this["username"] = value;
            }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                this["password"] = value;
            }
        }

        [ConfigurationProperty("port", IsRequired = false)]
        public int Port
        {
            get
            {
                return (int)this["port"];
            }
            set
            {
                this["port"] = value;
            }
        }
        
        [ConfigurationProperty("rulesConnectionString", IsRequired = false)]
        public string RulesConnectionString
        {
            get
            {
                return (string)this["rulesConnectionString"];
            }
            set
            {
                this["rulesConnectionString"] = value;
            }
        }
    }
}
