using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMagora.Logger;


namespace WebMagora.Controllers
{
    public class ErrorController : Controller
    {
        void logged404()
        {
            logger.grab.Info(
                new
                {
                    ip = Request.UserHostAddress,
                    url = Request.RawUrl,
                    url_referrer = (Request.UrlReferrer != null) ? Request.UrlReferrer.AbsolutePath : "",
                    browser = Request.Browser.Browser,
                    useragent = Request.UserAgent
                });
        }
        protected override void HandleUnknownAction(string actionName)
        {
            logged404();
            this.RouteData.Values["controller"] = "error";
            this.View("Error404").ExecuteResult(this.ControllerContext);
        }
        public ActionResult Init()
        {
            if (RouteData.DataTokens.Keys.Where(k => k.Equals("httpStatusCode")).Count() > 0)
            {
                int httpStatusCode = Convert.ToInt32(RouteData.Values["httpStatusCode"]);
                return General(httpStatusCode);
            }
            return HttpError404();
        }

        [PreventDirectAccess]
        public ActionResult General(int httpStatusCode)
        {
            RouteData.Values["controller"] = "error";
            return View("Error", httpStatusCode);
        }


        [PreventDirectAccess]
        public ActionResult HttpError403()
        {
            RouteData.Values["controller"] = "error";
            return View("Error403");
        }

        public ActionResult HttpError404()
        {
            logged404();
            RouteData.Values["controller"] = "error";
            System.Web.HttpContext.Current.Response.Cookies.Clear();
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.StatusCode = 404;
            return View("Error404");
        }

        public ActionResult HttpError500()
        {
            RouteData.Values["controller"] = "error";
            return View("Error500");
        }

        private class PreventDirectAccessAttribute : FilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationContext filterContext)
            {
                object value = filterContext.RouteData.Values["fromAppErrorEvent"];
                if (!(value is bool && (bool)value))
                    filterContext.Result = new ViewResult { ViewName = "Error404" };
            }
        }
    }
}
