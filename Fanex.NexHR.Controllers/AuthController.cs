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
    using Microsoft.AspNetCore.Mvc;
    using Novell.Directory.Ldap;

    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly ISessionService sessionService;
        private readonly ICryptoService cryptoService;
        private readonly AppSetting appSetting;
        private readonly LdapSettings ldapSettings;

        public AuthController(IAuthService authService,
            ISessionService sessionService,
            ICryptoService cryptoService,
            AppSetting appSetting,
            LdapSettings ldapSettings)
        {
            this.authService = authService;
            this.sessionService = sessionService;
            this.cryptoService = cryptoService;
            this.appSetting = appSetting;
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

        [HttpGet]
        public async Task<IActionResult> TestKey()
        {
            return Json(cryptoService.RsaEncrypt("1") + "----" + cryptoService.RsaDecrypt("9wnrJBPycdT2WLFbFFtA9w=="));
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (await IsLoginSuccess(model))
                {
                    string domainUsername = appSetting.DomainName + model.Username.Trim();
                    var employee = await authService.GetEmployeeByDomain(domainUsername);
                    if (employee != null && employee.Enabled && !employee.IsDeleted)
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

        private async Task<bool> IsLoginSuccess(LoginModel model)
        {
            return appSetting.LoginType == 1 ? IsLoginLdapSuccess(model) : await IsLoginByPasswordSuccess(model);
        }

        private async Task<bool> IsLoginByPasswordSuccess(LoginModel model)
        {
            var password = cryptoService.RsaEncrypt(model.Password);
            var isLoginSuccess = await authService.IsLoginSuccess(model.Username, password);
            return isLoginSuccess;
        }

        private bool IsLoginLdapSuccess(LoginModel model)
        {
            var isValid = false;
            using (var connection = new LdapConnection())
            {
                try
                {
                    connection.Connect(ldapSettings.Host, ldapSettings.Port);
                    connection.Bind(string.Format(ldapSettings.UserLoginSchema, model.Username), model.Password);
                    isValid = connection.Bound;
                }
                catch
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