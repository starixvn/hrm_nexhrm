namespace Fanex.NexHR.Controllers
{
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

        public AuthController(IAuthService authService,
            ISessionService sessionService,
            AppSetting appSetting,
            IWebHostEnvironment webHostEnvironment)
        {
            this.authService = authService;
            this.sessionService = sessionService;
            this.appSetting = appSetting;
            this.webHostEnvironment = webHostEnvironment;
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
                if (IsValidAccount(model))
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
                                new Claim(ClaimTypes.Name, model.Username),
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
                        ModelState.AddModelError("Username", Messages.UserNotExists);
                    }
                }
                else
                {
                    ViewBag.ReturnUrl = returnUrl;
                    ModelState.AddModelError("Username", Messages.IncorrectUsernameOrPassword);
                }
            }
            return View(appSetting.LoginView, model);
        }

        private bool IsValidAccount(LoginModel model)
        {
            if (webHostEnvironment.IsDevelopment())
            {
                return true;
            }

            string userDn = $"{model.Username}@{appSetting.DomainName.Replace("\\", "")}";
            var isValid = false;
            using (var connection = new LdapConnection())
            {
                connection.Connect(appSetting.LdapUrl, LdapConnection.DefaultPort);
                try
                {
                    connection.Bind(userDn, model.Password);
                    if (connection.Bound)
                    {
                        isValid = true;
                    }
                }
                catch
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public IActionResult Logout()
        {
            sessionService.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}