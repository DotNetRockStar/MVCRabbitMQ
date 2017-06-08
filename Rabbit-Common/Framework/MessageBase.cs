using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework
{
    /// <summary>
    /// Class that all messages have to inherit to work with the framework pieces created.
    /// </summary>
    public abstract class MessageBase
    {
        public MessageBase()
        {
            this.DateCreated = DateTime.UtcNow;
        }

        public DateTime DateCreated { get; internal set; }
    }
}
