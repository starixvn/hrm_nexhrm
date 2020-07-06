namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.ViewModels;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NexHR.Resources;

    public class AuthController : Controller
    {
        readonly MembershipProvider domainProvider = Membership.Providers["ADMembershipProvider"];

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
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
                        //HttpContext.Session.Abandon();
                        //HttpContext.Session.Clear();
                        string domainUsername = AppSetting.DomainName + model.UserName;
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

        [AllowAnonymous]
        public ActionResult Logout()
        {
            HttpContext.Session.Abandon();
            HttpContext.Session.RemoveAll();

            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}