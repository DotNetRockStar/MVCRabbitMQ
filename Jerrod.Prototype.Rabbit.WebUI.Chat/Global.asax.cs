using Jerrod.RabbitCommon;
using Jerrod.RabbitCommon.Framework;
using Jerrod.RabbitCommon.Framework.Data;
using Jerrod.RabbitCommon.Messages;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Jerrod.Prototype.Rabbit.WebUI.Chat
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        public static Publisher<ChatMessage> Publisher = null;

        protected void Application_Start()
        {
            #region Normal MVC Stuff

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            #endregion

            // Set up the rules repository that should be used for driving team rules.
            RegistrationUtility.ResolveRules = (controller) =>
            {
                return new AlwaysWorkRulesRepository();
            };
            
            // Create settings for publishing messages
            var uniqueQueueSettingsForThisAppInstance = new Settings() { QueueName = "Jerrod.RabbitCommon.Messages." + Guid.NewGuid().ToString("N") };
            var exchange = new Exchange("Jerrod.Prototype.Rabbit.UI.Chat", ExchangeType.Fanout);

            // Create messaging publisher.
            Publisher = new Publisher<ChatMessage>(uniqueQueueSettingsForThisAppInstance, exchange);

            // Initialize new listener.  No need to keep variable in this case, it will run in the background app in the App Pool.
            new SignalRListener(uniqueQueueSettingsForThisAppInstance, exchange, GlobalHost.ConnectionManager.GetHubContext<ChatHub>().Clients);
        }

        protected void Application_End()
        {
            ExchangeManager.CloseAllCreatedExchanges();
            QueueManager.ForceCloseAllQueues();
        }
    }

    public class AlwaysWorkRulesRepository : IRulesRepository
    {

        public bool IsQueueApproved(string host, string queueName)
        {
            return true;
        }

        public bool AreServerRulesEnforced(string host)
        {
            return true;
        }

        public bool IsExchangeApproved(string host, string name, RabbitCommon.ExchangeType type)
        {
            return true;
        }
    }
}