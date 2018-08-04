using System.Web.Mvc;

namespace Phoenix.Filters
{
    public class ExceptionLogAttribute : FilterAttribute, IExceptionFilter
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception;


            logger.Fatal(exception, $"Unhandled Exception!");
        }
    }
}