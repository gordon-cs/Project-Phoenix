using Phoenix.Services;
using System;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Phoenix.Filters
{
    /// <summary>
    /// Log any uncaught exceptions in controller classes.
    /// </summary>
    public class ExceptionLogAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            LoggerService log = new LoggerService();
            StringBuilder message  = new StringBuilder();

            // Unsolicited information: The user agent string is a mess.
            // Here is an article to get you up to date:
            // http://webaim.org/blog/user-agent-string-history/
            // FUN
            message.AppendLine("User-Agent: " + filterContext.HttpContext.Request.UserAgent);
            message.Append(filterContext.HttpContext.Request.UserHostAddress);
            message.Append(" ");
            message.Append(filterContext.HttpContext.Request.HttpMethod.ToString());
            message.Append(" ");
            message.AppendLine(filterContext.HttpContext.Request.Url.ToString());
            message.AppendLine("Exception: " + filterContext.Exception.Message);
            message.AppendLine("Inner Exception: " + filterContext.Exception.InnerException);


            log.Error(message.ToString());
        }
    }
}