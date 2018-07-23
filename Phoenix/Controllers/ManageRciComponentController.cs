using Phoenix.Filters;
using Phoenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Phoenix.Controllers
{
    [CustomAuthentication]
    [Admin]
    public class ManageRciComponentController : Controller
    {
    }
}