using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    public static class QueueExtensions
    {
        /// <summary>
        /// Delete any queue on the channel specified.
        /// </summary>
        /// <param name="channel">The channel for which the queue must exist to be deleted.</param>
        /// <param name="queueName">The name of the queue to be deleted.</param>
        /// <returns></returns>
        internal static uint DeleteQueue(this IModel channel, string queueName)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");

            return channel.QueueDelete(queueName);
        }

        /// <summary>
        /// Delete queue on specified channel.
        /// </summary>
        /// <param name="channel">The channel for which the queue must exist to be deleted.</param>
        /// <param name="queueName">The name of the queue to be deleted.</param>
        /// <param name="onlyIfEmpty">If specified to true then it will only delete the queue if the queue is empty.</param>
        /// <param name="onlyIfUnused">If specified to true then it will only delete the queue if the queue is unused.</param>
        /// <returns></returns>
        internal static uint DeleteQueue(this IModel channel, string queueName, bool onlyIfEmpty, bool onlyIfUnused)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");

            return channel.QueueDelete(queueName, onlyIfUnused, onlyIfEmpty);
        }

        public static uint DeleteQueue(this IQueueCreator queueCreator)
        {
            if (queueCreator == null || queueCreator.Channel == null)
                return default(uint);

            return queueCreator.Channel.QueueDelete(queueCreator.QueueName);
        }
    }
}
