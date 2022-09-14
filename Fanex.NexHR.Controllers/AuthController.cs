namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Hosting;
    using Novell.Directory.Ldap;

    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly ISessionService sessionService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly AppSetting appSetting;
        private readonly LdapSettings ldapSettings;

        public AuthController(IAuthService authService,
            ISessionService sessionService,
            AppSetting appSetting,
            IWebHostEnvironment webHostEnvironment,
            LdapSettings ldapSettings)
        {
            this.authService = authService;
            this.sessionService = sessionService;
            this.appSetting = appSetting;
            this.webHostEnvironment = webHostEnvironment;
            this.ldapSettings = ldapSettings;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string domainUsername = HttpContext.User.Identity.Name;
                var employee = await authService.GetEmployeeByDomain(domainUsername);

                if (employee.Enabled && !employee.IsDeleted)
                {
                    RedirectToAction("Index", "Dashboard");
                }
            }

            return View(appSetting.LoginView);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (IsLoginLdapSuccess(model))
                {
                    string domainUsername = appSetting.DomainName + model.Username;
                    var employee = await authService.GetEmployeeByDomain(domainUsername);
                    if (employee != null)
                    {
                        var employeeSetting = await authService.GetEmployeeSetting(employee.Id);
                        sessionService.CurrentEmployee = employee;
                        sessionService.CurrentUserSetting = employeeSetting;

                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, domainUsername),
                                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString())
                            };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            IsPersistent = model.RememberMe
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(identity), authProperties);

                        return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        ViewBag.ReturnUrl = returnUrl;
                        ViewBag.ErrorMessage = Messages.UserNotExists;
                    }
                }
                else
                {
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.ErrorMessage = Messages.IncorrectUsernameOrPassword;
                }
            }
            return View(appSetting.LoginView, model);
        }

        private bool IsLoginLdapSuccess(LoginModel model)
        {
            if (webHostEnvironment.IsDevelopment())
            {
                return true;
            }

            var isValid = false;
            using (var connection = new LdapConnection())
            {
                connection.Connect(ldapSettings.Host, ldapSettings.Port);
                try
                {
                    connection.Bind(string.Format(ldapSettings.UserLoginSchema, model.Username), model.Password);
                    if (connection.Bound)
                    {
                        isValid = true;
                    }
                }
                catch (Exception ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public async Task<IActionResult> Logout()
        {
            sessionService.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}