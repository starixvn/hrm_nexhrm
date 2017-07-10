// --------------------------------------------------------------------------
// <copyright file="BaseController.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Controllers
{
    using Criteria.Employee;
    using Models;
    using Services;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;

    [Authorize]
    public class BaseController : Controller
    {
        public Employee CurrentUser
        {
            get
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                Employee currentUser = (Employee)HttpContext.Session?["CurrentUser"];

                if (currentUser == null)
                {
                    var getEmployeeCriteria = new GetEmployeeByDomainNameCriteria { DomainUserName = HttpContext.User.Identity.Name };
                    currentUser = new EmployeeService().GetEmployeeByDomainName(getEmployeeCriteria);
                    if (currentUser != null)
                    {
                        if (!currentUser.Enabled || currentUser.IsDeleted)
                        {
                            Response.Redirect("~/home/accountsuspended", true);
                        }

                        if (HttpContext.Session != null)
                        {
                            HttpContext.Session["CurrentUser"] = currentUser;
                        }
                    }
                    else
                    {
                        FormsAuthentication.RedirectToLoginPage();
                    }
                }

                return currentUser;
            }
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext != null)
            {
                var attributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                if (attributes.Length == 0)
                {
                    attributes = GetType().GetCustomAttributes(typeof(AuthorizeAttribute), true);
                }

                if (attributes.Length == 0)
                {
                    return;
                }

                foreach (AuthorizeAttribute item in attributes)
                {
                    if (CurrentUser.IsSpecialUser)
                    {
                        if (!item.Roles.Contains("SpecialUser"))
                        {
                            filterContext.Result = new RedirectResult("~/home/accessdenied?url=" + Request.Url?.AbsoluteUri);
                        }
                    }
                    else
                    {
                        item.Roles = item.Roles.Replace("SpecialUser", string.Empty);
                        if (!string.IsNullOrEmpty(item.Roles)
                            && !CurrentUser.IsInRoles(item.Roles.Split(',').ToList()))
                        {
                            filterContext.Result = new RedirectResult("~/home/accessdenied?url=" + Request.Url?.AbsoluteUri);
                        }
                    }
                }
            }
        }
    }
}
