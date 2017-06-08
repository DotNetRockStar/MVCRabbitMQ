using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework.Data
{
    /// <summary>
    /// The rules repository used to determine if certain things pass rules set.
    /// </summary>
    public interface IRulesRepository
    {
        /// <summary>
        /// Returns true if the queue is approved and false if it is not.
        /// </summary>
        /// <param name="host">The host for the queue.  You can have queues on different servers and this should be the hostname for the server.</param>
        /// <param name="queueName">The name of the queue being asked about.</param>
        /// <returns>true if is approved, false if it is not.</returns>
        bool IsQueueApproved(string host, string queueName);
        /// <summary>
        /// Determines if server rules are enforced for a host.
        /// </summary>
        /// <param name="host">Hostname for a RabbitMQ server.</param>
        /// <returns>True if rules are enforced, false if they are not.</returns>
        bool AreServerRulesEnforced(string host);
        /// <summary>
        /// Returns true if the exchange is approved and false if it is not.
        /// </summary>
        /// <param name="host">The host for the queue.  You can have queues on different servers and this should be the hostname for the server.</param>
        /// <param name="name">The name of the exchange being asked about.</param>
        /// <param name="type">The exchange type being asked about.</param>
        /// <returns>true if is approved, false if it is not.</returns>
        bool IsExchangeApproved(string host, string name, ExchangeType type);
    }
}
