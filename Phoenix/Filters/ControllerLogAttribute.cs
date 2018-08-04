using System.Text;
using System.Web.Mvc;

namespace Phoenix.Filters
{
    public class ControllerLogAttribute : FilterAttribute, IActionFilter
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            var controllerActionName = filterContext.ActionDescriptor.ActionName;

            var actionArguments = filterContext.ActionParameters;

            var user = filterContext.Controller.TempData["user"];

            var logStatement = new StringBuilder();

            logStatement.Append($"User {user} at {controllerName}.{controllerActionName}. Arguments: ");

            foreach (var kvp in actionArguments)
            {
                // Try not to log passwords lol
                if (kvp.Key.Equals("password", System.StringComparison.OrdinalIgnoreCase)) { continue; }

                logStatement.Append($"{kvp.Key}={kvp.Value} ");
            }

            logger.Info(logStatement.ToString());
        }
    }
}