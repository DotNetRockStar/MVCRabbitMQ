using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework.Data.Entities
{
    public class Queue
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsApproved { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateApprovalEnds { get; set; }
        public DateTime? DateApprovalBegins { get; set; }
    }
}
