using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Phoenix.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        /// <summary>
        /// To delete a cookie, you set its expiration value to a date in the past. When the browswer sees the past
        /// date, it will remove the cookie.
        /// </summary>
        /// <returns></returns>
        public void Index()
        {
            TempData.Remove("user");
            TempData.Remove("admin");
            TempData.Remove("role");
            TempData.Remove("id");
            TempData.Remove("currentBuilding");
            TempData.Remove("currentRoom");
            TempData.Remove("kingdom");
            var oldCookie = Response.Cookies.Get("Authentication");
            // Set Expires to "1999/1/1 00:00:00", a random past date.
            oldCookie.Expires = new DateTime(1999, 1, 1, 0, 0, 0);
            Response.Cookies.Set(oldCookie);
        }
    }
}