using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    /// <summary>
    /// Configures how a listener receives messages.
    /// https://www.rabbitmq.com/consumer-prefetch.html
    /// </summary>
    public class Qos
    {
        public uint PrefetchSize { get; set; }
        public ushort PrefetchCount { get; set; }
        public bool IsGlobal { get; set; }

        public Qos()
        {
            this.PrefetchSize = 0;
            this.PrefetchCount = 1;
            this.IsGlobal = false;
        }
    }
}
