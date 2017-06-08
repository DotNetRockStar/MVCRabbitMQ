using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework.Data
{
    public class AllowAllRulesRepository : IRulesRepository
    {
        public bool AreServerRulesEnforced(string host)
        {
            return true;
        }

        public bool IsExchangeApproved(string host, string name, ExchangeType type)
        {
            return true;
        }

        public bool IsQueueApproved(string host, string queueName)
        {
            return true;
        }
    }
}
