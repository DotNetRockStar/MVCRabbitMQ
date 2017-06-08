using Jerrod.RabbitCommon.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Messages
{
    public class ChatMessage : MessageBase
    {
        public string CreatedBy { get; set; }
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
