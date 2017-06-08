using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework
{
    /// <summary>
    /// Attribute that is required on a RabbitController method to signify that the method should be invoked when the message in the input parameter is retrieved.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RabbitMethodAttribute : Attribute
    {
        /// <summary>
        /// Override the routing key for the message that is being listened for.  By default this will be the full type name of the message in the input parameter.
        /// </summary>
        public string RoutingKey { get; set; }

        public RabbitMethodAttribute() { }
    }
}
