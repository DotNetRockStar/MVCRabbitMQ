using Jerrod.RabbitCommon.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Messages
{
    public class VoidFrameworkTestMessage : MessageBase
    {
        public string SecretMessage { get; set; }
    }
}
