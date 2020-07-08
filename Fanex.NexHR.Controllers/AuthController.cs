namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.ViewModels;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Services;

    public class AuthController : Controller
    {
        readonly MembershipProvider domainProvider = Membership.Providers["ADMembershipProvider"];
        private readonly IAuthService authService;
        private readonly ISessionService sessionService;

        public AuthController()
        {
            authService = new AuthService();
            sessionService = new SessionService();
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
                if (domainProvider.ValidateUser(model.UserName, model.Password))
                {
                    var user = domainProvider.GetUser(model.UserName, false);
                    if (user.IsApproved)
                    {
                        string domainUsername = AppSetting.DomainName + model.UserName;
                        var employee = authService.GetEmployeeByDomain(domainUsername);
                        var employeeSetting = authService.GetEmployeeSetting(employee.Id);
                        sessionService.CurrentEmployee = employee;
                        sessionService.CurrentUserSetting = employeeSetting;
                        FormsAuthentication.SetAuthCookie(domainUsername, model.RememberMe);
                        return Url.IsLocalUrl(ReturnUrl) ? Redirect(ReturnUrl) : (ActionResult)RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("errorSummary", Messages.AccountLockedOut);
                    }
                }
                else
                {
                    ModelState.AddModelError("errorSummary", Messages.IncorrectUsernameOrPassword);
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            sessionService.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}