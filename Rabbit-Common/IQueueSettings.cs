using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    /// <summary>
    /// RabbitMQ settings used for creating queues.
    /// </summary>
    public interface IQueueSettings
    {
        string QueueName { get; set; }
        string RoutingKey { get; set; }
        bool IsDurable { get; set; }
        bool IsExclusive { get; set; }
        bool AutoDelete { get; set; }
        IDictionary<string, object> Arguments { get; set; }
    }
}
