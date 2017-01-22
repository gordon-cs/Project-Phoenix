using Phoenix.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Phoenix.Controllers
{
    public class PlaceholderController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {      
            return View();
        }
    }
}