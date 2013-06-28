using System.Web;
using System.Web.Mvc;
using WebMagora.Filters;

namespace WebMagora
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MagoraHandleErrorAttribute());
        }
    }
}