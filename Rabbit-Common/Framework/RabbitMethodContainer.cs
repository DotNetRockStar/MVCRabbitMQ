using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Framework
{
    internal class RabbitMethodContainer<TMessage, TResult> : IListenerInvoker<TMessage, TResult>
        where TMessage : MessageBase
    {
        private MethodInfo _methodInfo;
        private RabbitController _controller;
        private readonly List<ActionFilterAttribute> _actionFilters;

        public RabbitMethodContainer(RabbitController controller, MethodInfo method)
        {
            this._methodInfo = method;
            this._controller = controller;

            var attributes = method.GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                if (attribute is ActionFilterAttribute)
                {
                    if (_actionFilters == null) _actionFilters = new List<ActionFilterAttribute>();

                    var actionFilter = attribute as ActionFilterAttribute;
                    _actionFilters.Add(actionFilter);
                }
            }

            attributes = controller.GetType().GetCustomAttributes(true); 
            foreach (var attribute in attributes)
            {
                if (attribute is ActionFilterAttribute)
                {
                    if (_actionFilters == null) _actionFilters = new List<ActionFilterAttribute>();

                    var actionFilter = attribute as ActionFilterAttribute;
                    _actionFilters.Add(actionFilter);
                }
            }
        }

        public TResult Execute(TMessage message)
        {
            bool cancelExecution = false;
            object result = null;

            // pre execute.
            if (_actionFilters != null)
                foreach (var filter in _actionFilters)
                {
                    var args = new OnActionExecutingEventArgs(_methodInfo, message, _controller);
                    try
                    {
                        filter.OnActionExecuting(args);
                        if (args._cancelExecution)
                            cancelExecution = true;
                    }
                    catch { }
                }

            // execute
            if (!cancelExecution)
                try
                {
                    result = _methodInfo.Invoke(_controller, new object[] { message });
                }
                catch (Exception ex)
                {
                    // exception
                    if (_actionFilters != null)
                    {
                        foreach (var filter in _actionFilters)
                        {
                            try
                            {
                                OnExceptionEventArgs args = new OnExceptionEventArgs(_methodInfo, message, _controller, ex);
                                filter.OnException(args);
                            }
                            catch { }
                        }
                    }
                }

            // post execute
            if (_actionFilters != null)
                foreach (var filter in _actionFilters)
                {
                    var args = new OnActionExecutedEventArgs(_methodInfo, message, _controller, result);
                    filter.OnActionExecuted(args);
                }

            if (result == null)
                return default(TResult);
            return (TResult)result;
        }
    }
}
