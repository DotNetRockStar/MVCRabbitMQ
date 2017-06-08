using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework
{
    /// <summary>
    /// Controller class that all controller's must inherit for methods to be properly invoked when a published message is received.
    /// </summary>
    public abstract class RabbitController
    {
        /// <summary>
        /// The rabbit server settings assigned to this controller.  These settings are the settings that should be used to connect to the RabbitMQ server.  If this
        /// is not specified in the constructor then the settings will be looked for in the app.config/web.config.
        /// </summary>
        public IRabbitServerSettings Settings { get; private set; }
        /// <summary>
        /// The exchange that should be used for listening and publishing messages.  If the exchange is not specified in the constructor then a default exchange will be used.
        /// This can be overriden for publishing by specifying the exchange details in the RabbitMethod attribute.
        /// </summary>
        public Exchange Exchange { get; private set; }
        
        /// <summary>
        /// Create default instance of the RabbitController with default settings and exchange.
        /// </summary>
        public RabbitController()
        {
            if (!ConfigSectionUtility.IsConfigSectionValid)
                throw new Exception("Unable to create instance of " + this.GetType().FullName + " .  Make sure that the RabbitConfigSection is registered and valid in your config file and try again.");
            Settings = new ConfigRabbitServerSettings();
        }
        /// <summary>
        /// Create instance of the RabbitController with a specific exchange.
        /// </summary>
        /// <param name="exchange">The exchange that will be used to listen for messages and re-publish messages.  If this is not set then a default will be used.  Publishing of messsages can be overriden by specifying exchange details in RabbitMethod attribute.</param>
        public RabbitController(Exchange exchange)
            : this()
        {
            if (exchange == null)
                throw new ArgumentNullException("exchange");

            this.Exchange = exchange;
        }
        /// <summary>
        /// Create instance of the RabbitController with specific server settings.
        /// </summary>
        /// <param name="settings">The server settings to be used to connect to the RabbitMQ server.</param>
        public RabbitController(IRabbitServerSettings settings)
            : this()
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            Settings = settings;
        }
        /// <summary>
        /// Create instance of the RabbitController with a specific exchange and specific server settings.
        /// </summary>
        /// <param name="exchange">The exchange that will be used to listen for messages and re-publish messages.  If this is not set then a default will be used.  Publishing of messsages can be overriden by specifying exchange details in RabbitMethod attribute.</param>
        /// <param name="settings">The server settings to be used to connect to the RabbitMQ server.</param>
        public RabbitController(Exchange exchange, IRabbitServerSettings settings)
            : this(exchange)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            Settings = settings;
        }
    }
}
