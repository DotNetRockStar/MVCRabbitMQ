using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework.Data
{
    /// <summary>
    /// Internal class (mostly used for testing) repository used to drive rules.
    /// </summary>
    internal class DefaultRulesRepository : IRulesRepository
    {
        private readonly Dictionary<string, List<string>> ApprovedQueues;
        private readonly Dictionary<string, List<Exchange>> ApprovedExchanges;

        public DefaultRulesRepository()
        {
            List<string> approvedQueueList = new List<string>()
            {
                "Jerrod.RabbitCommon.Messages.FrameworkTestMessage",
                "Jerrod.RabbitCommon.Messages.FrameworkTestResponse"
            };

            ApprovedQueues = new Dictionary<string,List<string>>()
            {
                { "10.160.147.196", approvedQueueList }
            };
        }

        public bool IsQueueApproved(string host, string queueName)
        {
            if (ApprovedQueues == null || !ApprovedQueues.ContainsKey(host))
                return false;

            var queues = ApprovedQueues[host];
            return queues.Any(d => d.ToLower() == queueName.ToLower());
        }

        public bool AreServerRulesEnforced(string host)
        {
            return false;
        }

        public bool IsExchangeApproved(string host, string name, ExchangeType type)
        {
            if (ApprovedExchanges == null || !ApprovedExchanges.ContainsKey(host))
                return false;

            var exchanges = ApprovedExchanges[host];
            return exchanges.Any(d => d.Name.ToLower() == name.ToLower() && d.Type == type);
        }
    }
}
