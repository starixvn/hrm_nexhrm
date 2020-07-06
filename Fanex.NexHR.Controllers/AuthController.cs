namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.ViewModels;
    using System.Web.Mvc;
    using System.Web.Security;

    public class AuthController : Controller
    {
        readonly MembershipProvider domainProvider = Membership.Providers["ADMembershipProvider"];

        [HttpGet]
        public ActionResult Login()
        {
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
                        HttpContext.Session.Abandon();
                        HttpContext.Session.Clear();
                        string domainUsername = AppSetting.DomainName + model.UserName;
                        FormsAuthentication.SetAuthCookie(domainUsername, model.RememberMe);
                        return Url.IsLocalUrl(ReturnUrl) ? Redirect(ReturnUrl) : (ActionResult)RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("errorSummary", "Account is currently locked out");
                    }
                }
                else
                {
                    ModelState.AddModelError("errorSummary", "Incorrect User name or Password");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Abandon();
            HttpContext.Session.Clear();

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}