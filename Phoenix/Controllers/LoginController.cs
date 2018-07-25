using System;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices.AccountManagement;
using Phoenix.Models.ViewModels;
using Phoenix.Services;
using Phoenix.Exceptions;

namespace Phoenix.Controllers
{
    public class LoginController : Controller
    {
        // Global Variables
        private PrincipalContext _ADContext;
        private ILoginService loginService;

        public LoginController(ILoginService service)
        {
            this.loginService = service;
        }

        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            var role = (string)TempData["role"];

            if (role != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Authenticate(LoginViewModel loginViewModel)
        {

            // Get the username and password from the view model
            string username = loginViewModel.Username;
            string password = loginViewModel.Password;

            try
            {
                _ADContext = loginService.ConnectToADServer();
            }
            catch (Exception)
            {
                loginViewModel.ErrorMessage = "There was a problem connecting to the server. We are sorry. Please try again later or contact the project maintainer.";
                return View("Index", loginViewModel);
            }
      
            UserPrincipal userEntry;
            try
            {
                userEntry = loginService.FindUser(username, _ADContext);
            }
            catch(UserNotFoundException)
            {
                _ADContext.Dispose();
                loginViewModel.ErrorMessage = "Oh dear, it seems that username or password is invalid.";
                return View("Index", loginViewModel);
            }
            catch(ArgumentNullException)
            {
                _ADContext.Dispose();
                loginViewModel.ErrorMessage = "Oh dear, it seems that username or password is invalid.";
                return View("Index", loginViewModel);
            }

            if (loginService.IsValidUser(username, password, _ADContext))
            {
                // Generate token and attach to header
                string jwtToken;
                try
                {
                    jwtToken = loginService.GenerateToken(username, userEntry.EmployeeId);
                }
                catch(InvalidUserException)
                {
                    // e.g. user is a staff member.
                    _ADContext.Dispose();
                    loginViewModel.ErrorMessage = "Sorry, you are not a student or a member of the Residence Life Staff, so you do not have access to this system.";
                    return View("Index", loginViewModel);
                }

                HttpCookie tokenCookie = new HttpCookie("Authentication")
                {
                    Value = jwtToken,
                    Expires = DateTime.Now.AddHours(2.0)
                };

                Response.Cookies.Add(tokenCookie);

                _ADContext.Dispose();
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                loginViewModel.ErrorMessage = "Oh dear, it seems that username or password is invalid.";
                return View("Index", loginViewModel);
            }
        }
    }
}