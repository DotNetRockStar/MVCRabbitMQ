using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework
{
    /// <summary>
    /// A container that represents a registered RabbitController method and is used in the RegistrationUtility.
    /// </summary>
    internal class MethodContainer
    {
        public MethodInfo Method { get; set; }
        public RabbitController Controller { get; set; }
        public Type ReturnType { get; set; }
        public Type MessageType { get; set; }
        public IDisposable Listener { get; set; }
        public string RoutingKey { get; set; }
    }
}
