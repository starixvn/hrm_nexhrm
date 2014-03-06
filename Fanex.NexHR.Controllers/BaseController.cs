// --------------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Controllers
{
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;

    /// <summary>
    /// Base controller.
    /// </summary>
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

                Employee currentUser = (Employee)HttpContext.Session["CurrentUser"];

                if (currentUser == null)
                {
                    var getEmployeeCriteria = new GetEmployeeCriteria() { DomainUserName = HttpContext.User.Identity.Name };
                    currentUser = new EmployeeService().Get(getEmployeeCriteria);
                    if (currentUser != null)
                    {
                        if (!currentUser.Enabled || currentUser.IsDeleted)
                        {
                            Response.Redirect("~/home/accountsuspended", true);
                        }

                        HttpContext.Session["CurrentUser"] = currentUser;
                    }
                    else
                    {
                        FormsAuthentication.RedirectToLoginPage();
                    }
                }

                return currentUser;
            }
        }

        /// <summary>
        /// Authorization event handler.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
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
                if (item.Roles != string.Empty && !this.CurrentUser.IsInRoles(item.Roles.Split(new[] { ',' }).ToList()))
                {
                    filterContext.Result = new RedirectResult("~/home/accessdenied?url=" + Request.Url.AbsoluteUri);
                }
            }
        }
    }
}
