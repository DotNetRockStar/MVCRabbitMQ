using Jerrod.RabbitCommon.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Framework.Server.ActionFilters
{
    public class ExceptionActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnException(OnExceptionEventArgs e)
        {
            string str = "An exception occured!  " + e.Exception.ToString();
        }
    }
}
