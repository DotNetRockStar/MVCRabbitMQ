using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Messages
{
    public class GetItemByDescriptionMessage
    {
        public List<string> OrderCode { get; set; }
        public List<string> Sku { get; set; }
        public string SkuDescription { get; set; }
        public string ChassisDescription { get; set; }
        public string AllDescription { get; set; }
        public string CategoryRefiner { get; set; }
        public string Category { get; set; }
        public string AppliedRefinementIds { get; set; }
        public string NonAppliedRefinementId { get; set; }

        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string Select { get; set; }
        public string Sort { get; set; }
    }
}
