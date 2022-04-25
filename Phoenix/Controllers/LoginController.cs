using System;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices.AccountManagement;
using Phoenix.Models.ViewModels;
using Phoenix.Services;
using Phoenix.Exceptions;

namespace Phoenix.Controllers
{
    [Filters.ExceptionLog]
    public class LoginController : Controller
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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

            logger.Info("User {0} is attempting to log in....", username);

            try
            {
                _ADContext = loginService.ConnectToADServer();
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception while trying to connect to the Active Directory Server. User={username}");

                loginViewModel.ErrorMessage = "There was a problem connecting to the server. We are sorry. Please try again later or contact the project maintainer.";

                return View("Index", loginViewModel);
            }
      
            UserPrincipal userEntry;

            try
            {
                userEntry = loginService.FindUser(username, _ADContext);
            }
            catch(UserNotFoundException e)
            {
                logger.Error(e, $"User {username} was not found!");

                _ADContext.Dispose();

                loginViewModel.ErrorMessage = "Oh dear, it seems that username or password is invalid.";

                return View("Index", loginViewModel);
            }
            catch(ArgumentNullException e)
            {
                logger.Error(e, $"Username or Active Directory Context was null. Username={username}");

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
                catch (UserNotFoundException e)
                {
                    // e.g. user is not in accounts table for some reason
                    _ADContext.Dispose();

                    loginViewModel.ErrorMessage = "The username/password combination provided is not valid.";

                    logger.Error(e, $"Username={username} provided correct credientials but was not found in the Accounts table, so was denied login.");

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
                logger.Error($"The username/password combination provided was not valid. Username={username}");

                loginViewModel.ErrorMessage = "Oh dear, it seems that username or password is invalid.";

                return View("Index", loginViewModel);
            }
        }
    }
}