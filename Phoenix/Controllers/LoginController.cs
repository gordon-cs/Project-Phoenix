using System;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices.AccountManagement;
using Phoenix.Models.ViewModels;
using Phoenix.Services;
using Phoenix.Models;


namespace Phoenix.Controllers
{
    public class LoginController : Controller
    {
        // Global Variables
        private PrincipalContext _ADContext;
        private LoginService loginService;

        public LoginController()
        {
            loginService = new Services.LoginService();
        }

        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Authenticate(LoginViewModel loginViewModel)
        {

            // Get the username and password from the view model
            string username = loginViewModel.username;
            string password = loginViewModel.password;
            if (password == null)
            {
                // if user did not enter a password, set it to empty string, since this value must be non-null
                password = "";
            }

            if (!ModelState.IsValid)
            {
                loginViewModel.errorMessage = "Invalid model state.";
                return View("Index", loginViewModel);
            }
            else
            {
                try
                {
                    _ADContext = loginService.ConnectToADServer();
                }
                catch (Exception e)
                {
                    loginViewModel.errorMessage = "There was a problem connection to the server. We are sorry. Please try again later or contact a system administrator.";
                    return View("Index", loginViewModel);
                }
                if (_ADContext != null)
                {
                    var userEntry = loginService.FindUser(username, _ADContext);
                    if (userEntry == null)
                    {
                        _ADContext.Dispose();
                        loginViewModel.errorMessage = "Oh dear, it seems that username or password is invalid.";
                        return View("Index", loginViewModel);
                    }
                    // If user does exist in Active Directory, try to validate him or her
                    else
                    {
                        if (loginService.IsValidUser(username, password, _ADContext))
                        {

                            // I think we could add code here for authorization of admin, etc.

                            // Generate token and attach to header
                            var jwtToken = loginService.GenerateToken(username, userEntry.Name, userEntry.EmployeeId);
                            HttpCookie tokenCookie = new HttpCookie("Authentication");
                            tokenCookie.Value = jwtToken;
                            tokenCookie.Expires = DateTime.Now.AddHours(2.0);
                            Response.Cookies.Add(tokenCookie);

                            _ADContext.Dispose();
                            return RedirectToAction("Index", "Dashboard");
                        }
                        else
                        {
                            loginViewModel.errorMessage = "Oh dear, it seems that username or password is invalid.";
                            return View("Index", loginViewModel);
                        }
                    }
                }
                else
                {
                    loginViewModel.errorMessage = "Oh dear, it seems that username or password is invalid.";
                    return View("Index", loginViewModel);
                }
            }
        }
    }
}