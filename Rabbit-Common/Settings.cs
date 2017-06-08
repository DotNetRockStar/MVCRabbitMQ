using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    public class Settings : IQueueSettings
    {
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
        public bool IsDurable { get; set; }
        public bool IsExclusive { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
    }
}
