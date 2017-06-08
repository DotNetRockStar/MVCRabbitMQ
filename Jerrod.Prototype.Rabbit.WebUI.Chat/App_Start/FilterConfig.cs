using System.Web;
using System.Web.Mvc;

namespace Jerrod.Prototype.Rabbit.WebUI.Chat
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}