using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework
{
    /// <summary>
    /// Attribute that can be applied to any RabbitController or RabbitController method that allows for pre-execution, post-execution, and exception handling.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ActionFilterAttribute : Attribute
    {
        /// <summary>
        /// Will be invoked before the RabbitController method is invoked.
        /// </summary>
        /// <param name="e">Arguments containing information about the incoming message, the controller being invoked, and ability to get attributes from the method.</param>
        public virtual void OnActionExecuting(OnActionExecutingEventArgs e) { }
        /// <summary>
        /// Will be invoked after the RabbitController method is invoked.
        /// </summary>
        /// <param name="e">Arguments containing information about the incoming message, the controller being invoked, and ability to get attributes from the method.</param>
        public virtual void OnActionExecuted(OnActionExecutedEventArgs e) { }
        /// <summary>
        /// Will be invoked if there is an exception during the execution of the RabbitController method.
        /// </summary>
        /// <param name="e">Arguments containing information about the incoming message, the exception, and the controller being invoked, and ability to get attributes from the method.</param>
        public virtual void OnException(OnExceptionEventArgs e) { }
    }
}
