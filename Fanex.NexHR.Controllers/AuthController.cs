namespace Fanex.NexHR.Controllers;

using System.Collections.Generic;
using GO.UserControl.Client.Core.Session;
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
using GOServices = GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Models.Auth;
using Fanex.NexHR.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System;

public class AuthController : Controller
{
    private readonly IAuthService authService;
    private readonly ISessionService sessionService;
    private readonly AppSetting appSetting;
    private readonly LdapSettings ldapSettings;
    private readonly IUserControlSession userControlSession;
    private readonly GOServices.IAuthService goService;
    private readonly UserControlClientOptions options;
    private readonly GOServices.ICryptoService cryptoService;

        public AuthController(
            IAuthService authService,
        ISessionService sessionService,
        AppSetting appSetting,
        LdapSettings ldapSettings,
        IUserControlSession userControlSession,
        GOServices.IAuthService goService,
        IOptions<UserControlClientOptions> config,
        GOServices.ICryptoService cryptoService)
    {
        this.authService = authService;
        this.sessionService = sessionService;
        this.appSetting = appSetting;
        this.ldapSettings = ldapSettings;
        this.userControlSession = userControlSession;
        this.goService = goService;
        options = config.Value;
        this.cryptoService = cryptoService;
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
            if (await IsLoginSuccess(model))
            {
                string domainUsername = appSetting.DomainName + model.Username.Trim();
                var employee = await authService.GetEmployeeByDomain(domainUsername);
                if (employee != null && employee.Enabled && !employee.IsDeleted)
                {
                    var employeeSetting = await authService.GetEmployeeSetting(employee.Id);
                    sessionService.CurrentEmployee = employee;
                    sessionService.CurrentUserSetting = employeeSetting;
                    var token = cryptoService.GenerateRijndaelKeyIV();
                    var keyIv = token.Split(':');
                    var rijndaelKey = keyIv[0];
                    var rijndaelIv = keyIv[1];
                    var loginParam = new LoginParam
                    {
                        Username = cryptoService.Encrypt(options.Username, rijndaelKey, rijndaelIv),
                        Password = cryptoService.Encrypt(options.Password, rijndaelKey, rijndaelIv),
                        Token = token,
                        IP = GetClientIp(HttpContext),
                        Platform = HttpContext.Request.Headers["User-Agent"].ToString(),
                        BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
                        NetSessionId = HttpContext.Session.Id
                    };
                    var loginResult = await goService.LoginAsync(loginParam);
                    userControlSession.UserId = loginResult.UserId;
                    userControlSession.JwtToken = loginResult.JwtToken;

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

    private static string GetClientIp(HttpContext httpContext)
    {
        var clientIp = httpContext.Request.Headers["x-forwarded-for"].ToString();
        if (string.IsNullOrEmpty(clientIp))
        {
            clientIp = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
        else
        {
            var forwardedIps = clientIp.Split([','], StringSplitOptions.RemoveEmptyEntries);

            clientIp = forwardedIps[0];
        }

        return clientIp;
    }

    private async Task<bool> IsLoginSuccess(LoginModel model)
    {
        return appSetting.LoginType == 1 ? IsLoginLdapSuccess(model) : await IsLoginByPasswordSuccess(model);
    }

    private async Task<bool> IsLoginByPasswordSuccess(LoginModel model)
    {
        var loginEmployee = await authService.GetLoginEmployee(model.Username);
        return loginEmployee != null && BCrypt.Net.BCrypt.Verify(model.Password, loginEmployee.Password);
    }

    private bool IsLoginLdapSuccess(LoginModel model)
    {
            bool isValid;
            using var connection = new LdapConnection();
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

        return isValid;
    }

    public async Task<IActionResult> Logout()
    {
        sessionService.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}