using System.Web.Mvc;

namespace Phoenix.Models.ViewModels
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        [AllowHtml]
        public string Password { get; set; }
        public string ErrorMessage { get; set; }
    }
}