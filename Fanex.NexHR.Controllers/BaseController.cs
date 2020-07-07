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
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;
    using Criteria.Employee;
    using Fanex.NexHR.Core;
    using Models;
    using Services;

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
                            Response.Redirect("~/error/accountsuspended", true);
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

        public EmployeeSetting CurrentUserSetting
        {
            get
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                EmployeeSetting currentSetting = (EmployeeSetting)HttpContext.Session?["CurrentUserSetting"];

                if (currentSetting == null)
                {
                    var employeeSettingCriteria = new GetCurrentUserSettingCriteria { EmployeeId = CurrentUser.Id };
                    var baseService = new BaseService();
                    currentSetting = baseService.GetDataEntity<EmployeeSetting>(employeeSettingCriteria);

                    if (currentSetting != null)
                    {
                        if (HttpContext.Session != null)
                        {
                            HttpContext.Session["CurrentUserSetting"] = currentSetting;
                        }
                    }
                    else
                    {
                        FormsAuthentication.RedirectToLoginPage();
                    }
                }

                return currentSetting;
            }
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (CurrentUser == null)
            {
                string HRMName = AppSetting.HRMName;
                if(HRMName!= "NexHRM")
                {
                    filterContext.Result = new RedirectResult("~/Auth/Login");
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/error.html");
                }
            }

            if (filterContext == null)
            {
                return;
            }

            object[] attributes = GetAttributes(filterContext);

            if (attributes.Length == 0)
            {
                return;
            }
            if (CurrentUser != null)
            {
                CheckRoles(filterContext, attributes);
            }
        }

        private void CheckRoles(AuthorizationContext filterContext, object[] attributes)
        {
            foreach (AuthorizeAttribute item in attributes)
            {
                if (!string.IsNullOrEmpty(item.Roles)
                    && !Employee.IsInRoles(item.Roles.Split(',').ToList()))
                {
                    filterContext.Result = new RedirectResult("~/home/accessdenied?url=" + Request.Url?.AbsoluteUri);
                }
            }
        }

        private object[] GetAttributes(AuthorizationContext filterContext)
        {
            var attributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true);
            if (attributes.Length == 0)
            {
                attributes = GetType().GetCustomAttributes(typeof(AuthorizeAttribute), true);
            }

            return attributes;
        }
    }
}