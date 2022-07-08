namespace Fanex.NexHR.Models.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class HrmAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public string Roles { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var employee = context.HttpContext.Session.Get<Employee>("CurrentUser");

            if (!string.IsNullOrWhiteSpace(Roles))
            {
                var userRoles = employee.RoleList;
                List<string> roleList = new List<string>();
                foreach (var role in userRoles)
                {
                    roleList.Add(((Role)role).ToString());
                }

                var permissionRoles = Roles.Split(",");

                if (!IsValidRole(roleList, permissionRoles))
                {
                    UnauthorizedResponse(context);

                    //NoPermisssionResponse(context);
                }
            }

            if (employee is null || employee.Id == 0)
            {
                UnauthorizedResponse(context);
            }
        }

        private static bool IsValidRole(List<string> roleList, string[] permissionRoles)
        {
            bool isSARole = roleList.Any(r => r.Equals(Utilities.GetDescription(Role.SA), StringComparison.OrdinalIgnoreCase));

            if (!isSARole)
            {
                foreach (var permission in permissionRoles)
                {
                    if (roleList.Any(r => r.Equals(permission, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
            }

            return isSARole;
        }

        private static void UnauthorizedResponse(AuthorizationFilterContext context)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Result = new RedirectResult("/Auth/Logout");
        }

        private static void NoPermisssionResponse(AuthorizationFilterContext context)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Result = new RedirectResult("/Dashboard");
        }
    }
}