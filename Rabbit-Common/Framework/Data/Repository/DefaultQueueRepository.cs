using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework.Data.Repository
{
    internal class DefaultQueueRepository : DefaultRepositoryBase, IQueueRepository
    {
        public DefaultQueueRepository()
            : base() { }

        public bool IsQueueApproved(string name)
        {
            throw new NotImplementedException();
        }

        public Entities.Queue Get(string id)
        {
            throw new NotImplementedException();
        }

        public List<Entities.Queue> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(string id, Entities.Queue queue)
        {
            throw new NotImplementedException();
        }

        public Entities.Queue Create(Entities.Queue queue)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
