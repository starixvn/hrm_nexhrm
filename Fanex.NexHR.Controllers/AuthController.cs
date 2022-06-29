namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using System.DirectoryServices.Protocols;
    using System.Security.Claims;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly ISessionService sessionService;
        private readonly IConfiguration configuration;

        public AuthController(IAuthService authService, ISessionService sessionService, IConfiguration configuration)
        {
            this.authService = authService;
            this.sessionService = sessionService;
            this.configuration = configuration;
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string domainUsername = HttpContext.User.Identity.Name;
                var employee = authService.GetEmployeeByDomain(domainUsername);
                return !employee.Enabled || employee.IsDeleted ? View() : (ActionResult)RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                if (IsValidAccount(model))
                {
                    string domainUsername = configuration.GetSection("domain").Value + model.UserName;
                    var employee = authService.GetEmployeeByDomain(domainUsername);
                    if (employee != null)
                    {
                        var employeeSetting = authService.GetEmployeeSetting(employee.Id);
                        sessionService.CurrentEmployee = employee;
                        sessionService.CurrentUserSetting = employeeSetting;

                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, model.UserName),
                                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString())
                            };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            IsPersistent = model.RememberMe
                        };

                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(identity), authProperties);

                        return Url.IsLocalUrl(ReturnUrl) ? Redirect(ReturnUrl) : (ActionResult)RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("errorSummary", Messages.UserNotExists);
                    }
                }
                else
                {
                    ModelState.AddModelError("errorSummary", Messages.IncorrectUsernameOrPassword);
                }

                
            }
            return View(model);
        }

        private bool IsValidAccount(LoginModel model)
        {
            //string userDn = $"{model.UserName}@{AppSetting.LDAP.Replace("\\", "")}";
            //var isValid = false;
            //using (var connection = new LdapConnection())
            //{
            //    connection.Connect(AppSetting.LDAP, LdapConnection.DefaultPort);
            //    try
            //    {
            //        connection.Bind(userDn, model.Password);
            //        if (connection.Bound)
            //        {
            //            isValid = true;
            //        }
            //    }
            //    catch
            //    {
            //        isValid = false;
            //    }
            //}

            return true;
        }

        public ActionResult Logout()
        {
            sessionService.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}