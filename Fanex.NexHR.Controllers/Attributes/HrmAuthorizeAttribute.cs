namespace Fanex.NexHR.Controllers.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class HrmAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public string UserRoles { get; set; }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                UnauthorizedResponse(context);
            }
            else
            {
                await ResetSession(context);

                if (!string.IsNullOrWhiteSpace(UserRoles))
                {
                    ValidateRoles(context);
                }
            }
        }

        private void ValidateRoles(AuthorizationFilterContext context)
        {
            var sessionService = context.HttpContext.RequestServices.GetService<ISessionService>();

            var userRoles = sessionService.CurrentEmployee.RoleList;
            List<string> roleList = new List<string>();
            foreach (var role in userRoles)
            {
                roleList.Add(((Role)role).ToString());
            }

            var permissionRoles = GetPermissionRoles();

            if (!IsValidRole(roleList, permissionRoles))
            {
                UnauthorizedResponse(context);
            }
        }

        private string[] GetPermissionRoles()
        {
            var permissionRoles = UserRoles.Split(",");
            if (permissionRoles.Any(r => r.Equals(Role.AssistantLeader.ToString(), StringComparison.OrdinalIgnoreCase)) &&
                !permissionRoles.Any(r => r.Equals(Role.Leader.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                permissionRoles = permissionRoles.Concat(new[] { Role.Leader.ToString() }).ToArray();
            }
            return permissionRoles;
        }

        private static async Task ResetSession(AuthorizationFilterContext context)
        {
            var sessionService = context.HttpContext.RequestServices.GetService<ISessionService>();
            var authService = context.HttpContext.RequestServices.GetService<IAuthService>();

            if (sessionService.CurrentEmployee is null || sessionService.CurrentEmployee.Id <= 0)
            {
                var domainUsername = context.HttpContext.User.Identity.Name;
                var employee = await authService.GetEmployeeByDomain(domainUsername);
                var employeeSetting = await authService.GetEmployeeSetting(employee.Id);

                sessionService.CurrentEmployee = employee;
                sessionService.CurrentUserSetting = employeeSetting;
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
            context.Result = new RedirectToActionResult("Login", "Auth", null);
        }
    }
}