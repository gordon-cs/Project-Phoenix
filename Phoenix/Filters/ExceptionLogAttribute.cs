using System;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Phoenix.Filters
{
    public class ExceptionLogAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            string today = DateTime.Today.ToShortDateString().Replace("/", "_");

            // Msdn doesn't have enough documentation on DateTime, so I used:
            // http://www.c-sharpcorner.com/uploadfile/mahesh/working-with-datetime-using-C-Sharp/
            // to figure out the type of timestamp format I wanted
            string timestamp = DateTime.Today.ToString("G");

            string folderPath = "\\Logs\\";
            Directory.CreateDirectory(HostingEnvironment.MapPath(folderPath));

            var stream = File.AppendText(HostingEnvironment.MapPath(folderPath + today + ".log"));

            stream.Write(timestamp);
            stream.Write(" --- ");
            stream.Write("[ERROR]");
            stream.Write(" ");
            // Unsolicited information: The user agent string is a mess.
            // Here is an article to get you up to date:
            // http://webaim.org/blog/user-agent-string-history/
            // FUN
            stream.WriteLine(filterContext.HttpContext.Request.UserAgent);
            stream.Write(filterContext.HttpContext.Request.UserHostAddress);
            stream.Write(" ");
            stream.Write(filterContext.HttpContext.Request.HttpMethod.ToString());
            stream.Write(" ");
            stream.WriteLine(filterContext.HttpContext.Request.Url.ToString());

            stream.WriteLine("Exception: " + filterContext.Exception.Message);

            stream.WriteLine("Inner Exception: " + filterContext.Exception.InnerException);

            stream.WriteLine();
            stream.WriteLine();


            stream.Dispose();
        }
    }
}