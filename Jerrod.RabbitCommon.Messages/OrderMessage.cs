using Jerrod.RabbitCommon.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon
{
    public class OrderMessage : MessageBase
    {
        public string Id { get; set; }
        public decimal Total { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class ProductMessage : MessageBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Total { get; set; }
    }
}
