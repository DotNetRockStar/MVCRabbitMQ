using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    public static class QueueManager
    {
        internal static Dictionary<string, IModel> CreatedQueues { get; set; }

        static QueueManager()
        {
            CreatedQueues = new Dictionary<string, IModel>();
        }

        internal static void AddCreatedQueue(IModel channel, string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                return;
            if (channel == null)
                return;

            if (!CreatedQueues.ContainsKey(queueName))
                CreatedQueues.Add(queueName, channel);
        }

        public static void ForceCloseAllQueuesIfInactive()
        {
            foreach (var queue in CreatedQueues)
            {
                queue.Value.DeleteQueue(queue.Key, true, true);
            }
        }

        public static void ForceCloseAllQueues()
        {
            foreach (var queue in CreatedQueues)
            {
                queue.Value.DeleteQueue(queue.Key);
            }
        }
    }
}
