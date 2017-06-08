using Jerrod.RabbitCommon.Framework.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework.Data
{
    /// <summary>
    /// Administrative repository used to perform actions against queues.
    /// </summary>
    public interface IQueueRepository
    {
        Queue Get(string id);
        List<Queue> GetAll();
        void Update(string id, Queue queue);
        Queue Create(Queue queue);
        void Delete(string id);
    }
}
