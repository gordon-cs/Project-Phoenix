using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class LoginViewModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool invalidCredentials { get; set; }
    }
}