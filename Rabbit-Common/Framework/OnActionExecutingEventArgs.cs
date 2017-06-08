
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework
{
    public class OnActionExecutingEventArgs : EventArgs
    {
        private readonly MethodInfo _method;
        internal bool _cancelExecution = false;
        /// <summary>
        /// The incoming message.
        /// </summary>
        public MessageBase Message { get; private set; }
        /// <summary>
        /// The controller that the method belongs to that is being invoked.
        /// </summary>
        public RabbitController Controller { get; private set; }

        public OnActionExecutingEventArgs(MethodInfo method, MessageBase message, RabbitController controller)
        {
            this._method = method;
            this.Message = message;
            this.Controller = controller;
        }

        /// <summary>
        /// Returns custom attributes on the RabbitController method being executed.
        /// </summary>
        /// <typeparam name="T">The type of attribute to be returned.</typeparam>
        /// <param name="inherit">When set to true, this will attempt to retrieve attributes for the method and all methods it is overriding.</param>
        /// <returns>List of System.Attribute's of type T.</returns>
        public IEnumerable<T> GetCustomAttributes<T>(bool inherit) where T : Attribute
        {
            return _method.GetCustomAttributes<T>(inherit);
        }
        public void CancelExecution()
        {
            _cancelExecution = true;
        }
    }
}
