using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMagora.Logger
{
    public static class logger
    {
        //static readonly log4net.ILog sql = log4net.LogManager.GetLogger("sql");
        internal class overlaylog : log4net.ILog
        {
            log4net.ILog _internal;
            bool _nolog = false;
            Delegate _d;

            public overlaylog(string name, Action<Exception> d)
            {
                _internal = log4net.LogManager.GetLogger(name);
                _d = d;
            }
            public overlaylog(string name, Action<Exception, string> d)
            {
                _internal = log4net.LogManager.GetLogger(name);
                _d = d;
            }

            public overlaylog(string name, Action<Exception> d, bool no_log)
            {
                _internal = log4net.LogManager.GetLogger(name);
                _d = d;
                _nolog = no_log;
            }

            public overlaylog(string name, Action<Exception, string> d, bool no_log)
            {
                _internal = log4net.LogManager.GetLogger(name);
                _d = d;
                _nolog = no_log;
            }

            private bool _exc(object exc, params object[] ps)
            {
                if (exc is Exception)
                {
                    if (_d != null)
                    {
                        try
                        {
                            if (ps.Count() > 0 && _d is Action<Exception, string>)
                            {
                                (_d as Action<Exception, string>).Invoke(exc as Exception, ps[0] as string);
                            }
                            else
                                (_d as Action<Exception>).Invoke(exc as Exception);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    return true;
                }
                return false;
            }

            public void Debug(object message, Exception exception)
            {
                _exc(exception);
                if (!_nolog)
                    _internal.Debug(message, exception);
            }

            public void Debug(object message)
            {
                if (_exc(message) && _nolog) return;
                _internal.Debug(message);
            }

            public void DebugFormat(IFormatProvider provider, string format, params object[] args)
            {
                _internal.DebugFormat(provider, format, args);
            }

            public void DebugFormat(string format, object arg0, object arg1, object arg2)
            {
                _internal.DebugFormat(format, arg0, arg1, arg2);
            }

            public void DebugFormat(string format, object arg0, object arg1)
            {
                _internal.DebugFormat(format, arg0, arg1);
            }

            public void DebugFormat(string format, object arg0)
            {
                _internal.DebugFormat(format, arg0);
            }

            public void DebugFormat(string format, params object[] args)
            {
                _internal.DebugFormat(format, args);
            }

            public void Error(object message, Exception exception)
            {
                _exc(exception, message);
                if (!_nolog)
                    _internal.Error(message, exception);
            }

            public void Error(object message)
            {
                if (_exc(message) && _nolog) return;
                _internal.Error(message);
            }

            public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
            {
                _internal.ErrorFormat(provider, format, args);
            }

            public void ErrorFormat(string format, object arg0, object arg1, object arg2)
            {
                _internal.ErrorFormat(format, arg0, arg1, arg2);
            }

            public void ErrorFormat(string format, object arg0, object arg1)
            {
                _internal.ErrorFormat(format, arg0, arg1);
            }

            public void ErrorFormat(string format, object arg0)
            {
                _internal.ErrorFormat(format, arg0);
            }

            public void ErrorFormat(string format, params object[] args)
            {
                _internal.ErrorFormat(format, args);
            }

            public void Fatal(object message, Exception exception)
            {
                _exc(exception);
                if (!_nolog)
                    _internal.Fatal(message, exception);
            }

            public void Fatal(object message)
            {
                if (_exc(message) && _nolog) return;
                _internal.Fatal(message);
            }

            public void FatalFormat(IFormatProvider provider, string format, params object[] args)
            {
                _internal.FatalFormat(provider, format, args);
            }

            public void FatalFormat(string format, object arg0, object arg1, object arg2)
            {
                _internal.FatalFormat(format, arg0, arg1, arg2);
            }

            public void FatalFormat(string format, object arg0, object arg1)
            {
                _internal.FatalFormat(format, arg0, arg1);
            }

            public void FatalFormat(string format, object arg0)
            {
                _internal.FatalFormat(format, arg0);
            }

            public void FatalFormat(string format, params object[] args)
            {
                _internal.FatalFormat(format, args);
            }

            public void Info(object message, Exception exception)
            {
                _exc(exception);
                if (!_nolog)
                    _internal.Info(message, exception);
            }

            public void Info(object message)
            {
                if (_exc(message) && _nolog) return;
                _internal.Info(message);
            }

            public void InfoFormat(IFormatProvider provider, string format, params object[] args)
            {
                _internal.InfoFormat(provider, format, args);
            }

            public void InfoFormat(string format, object arg0, object arg1, object arg2)
            {
                _internal.InfoFormat(format, arg0, arg1, arg2);
            }

            public void InfoFormat(string format, object arg0, object arg1)
            {
                _internal.InfoFormat(format, arg0, arg1);
            }

            public void InfoFormat(string format, object arg0)
            {
                _internal.InfoFormat(format, arg0);
            }

            public void InfoFormat(string format, params object[] args)
            {
                _internal.InfoFormat(format, args);
            }

            public bool IsDebugEnabled
            {
                get { return _internal.IsDebugEnabled; }
            }

            public bool IsErrorEnabled
            {
                get { return _internal.IsErrorEnabled; }
            }

            public bool IsFatalEnabled
            {
                get { return _internal.IsFatalEnabled; }
            }

            public bool IsInfoEnabled
            {
                get { return _internal.IsInfoEnabled; }
            }

            public bool IsWarnEnabled
            {
                get { return _internal.IsWarnEnabled; }
            }

            public void Warn(object message, Exception exception)
            {
                _exc(exception);
                if (!_nolog)
                    _internal.Warn(message, exception);
            }

            public void Warn(object message)
            {
                if (_exc(message) && _nolog) return;
                _internal.Warn(message);
            }

            public void WarnFormat(IFormatProvider provider, string format, params object[] args)
            {
                _internal.WarnFormat(provider, format, args);
            }

            public void WarnFormat(string format, object arg0, object arg1, object arg2)
            {
                _internal.WarnFormat(format, arg0, arg1, arg2);
            }

            public void WarnFormat(string format, object arg0, object arg1)
            {
                _internal.WarnFormat(format, arg0, arg1);
            }

            public void WarnFormat(string format, object arg0)
            {
                _internal.WarnFormat(format, arg0);
            }

            public void WarnFormat(string format, params object[] args)
            {
                _internal.WarnFormat(format, args);
            }

            public log4net.Core.ILogger Logger
            {
                get { return _internal.Logger; }
            }
        }

        public static readonly log4net.ILog smtp = log4net.LogManager.GetLogger("smtp");
        private static string get_info()
        {
            HttpRequest request = HttpContext.Current.Request;
            string res = string.Empty;
            if (new string[] { "::1", "127.0.0.1" }.Contains(HttpContext.Current.Request.UserHostAddress)) return res;
            try
            {
                res = string.Format("Info HTTPclient:{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                    request.Browser.Browser,
                    request.UserHostAddress,
                    (request.UrlReferrer == null) ? "" : request.UrlReferrer.AbsolutePath,
                    request.UserAgent,                    
                    request.Browser.Platform,
                    request.Browser.Version
                    , request.RawUrl);
            }
            catch
            {
            }
            return res;
        }

        public static readonly log4net.ILog sql = new overlaylog("sql", (exc, m) => { if (m != null) smtp.Error(m, exc); else smtp.Error(exc); /*if (exc is System.Data.SqlClient.SqlException) throw exc;*/ });
        public static readonly log4net.ILog controller = new overlaylog("controller", exc => smtp.Error(exc));
        public static readonly log4net.ILog app = new overlaylog("app", exc => smtp.Error(get_info(), exc));//log4net.LogManager.GetLogger("app");
        public static readonly log4net.ILog grab = log4net.LogManager.GetLogger("grab");

    }
}