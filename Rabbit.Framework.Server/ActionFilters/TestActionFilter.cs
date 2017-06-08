using Jerrod.RabbitCommon.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Framework.Server.ActionFilters
{
    public class TestActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(OnActionExecutingEventArgs e)
        {
            var x = "it fired before the method did!!!";
        }

        public override void OnActionExecuted(OnActionExecutedEventArgs e)
        {
            var x = "it fired AFTER the method did!!!";
        }
    }
}
