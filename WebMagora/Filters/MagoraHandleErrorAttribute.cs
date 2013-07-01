using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebMagora.Logger;

namespace WebMagora.Filters
{  
    /// <summary>
    /// Обработчик ошибок в контроллерах
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MagoraHandleErrorAttribute: HandleErrorAttribute
    {
        internal class _ErrorInfo
        {
            public ErrorInfo Exception { get; set; }
            //  public ErrorInfo exception { get { return Exception; } }
        }
        internal class ErrorInfo
        {
            public string Type { get; set; }
            public string Message { get; set; }
            public string Url { get; set; }
            // public string type { get { return Type; } }
            //  public string message { get { return Message; } }

        }

        /// <summary>
        /// Проверка контекста на Ajax запрос
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        bool IsAjax(ExceptionContext c)
        {
            return (c.HttpContext.Request.Headers.Get("x-microsoftajax") ?? "")
                .Equals("delta=true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Обработка исключений
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is SqlException)
            {
                logger.sql.Error(context.Exception);
                if(IsAjax(context))
               {
	            SqlException exc_sql = (context.Exception as SqlException);
                
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
                context.Result = new ContentResult
                {
                    ContentType = "application/json",
                    Content = serializer.Serialize(
                    new _ErrorInfo() { Exception = new ErrorInfo { Type = "sql", Message = exc_sql.Message } }
                    )
                };
                context.Exception = null;
                context.ExceptionHandled = true;
               }
            }
            else
            {
                logger.controller.Error(context.Exception);
                if(IsAjax(context))
               {
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };

                context.Result = new ContentResult
                {
                    ContentType = "application/json",
                    Content = serializer.Serialize(
                    new _ErrorInfo()
                    {
                        Exception = new ErrorInfo
                        {
                            Type = "controller",
                            Message = context.Exception.ToString(),
                            Url = context.HttpContext.Request.RawUrl
                        }
                    }
                    )
                };
                context.ExceptionHandled = true;
	}
            }
        }
    }
}