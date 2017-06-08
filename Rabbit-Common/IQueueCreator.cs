using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    public interface IQueueCreator
    {
        IConnection Connection { get; }
        IModel Channel { get; }
        string QueueName { get; }
    }
}
