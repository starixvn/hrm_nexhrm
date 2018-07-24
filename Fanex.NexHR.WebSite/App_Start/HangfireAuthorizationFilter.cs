namespace Fanex.NexHR.WebSite.App_Start
{
    using System.Linq;
    using System.Web.Security;
    using Core;
    using Core.Enums;
    using Hangfire.Dashboard;
    using Microsoft.Owin;

    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var owinContext = new OwinContext(context.GetOwinEnvironment());

            return owinContext.Authentication.User.Identity.IsAuthenticated && IsInRoles(Role.HR);
        }

        private static bool IsInRoles(params Role[] roles)
        {
            var userRoles = Roles.GetRolesForUser();
            return roles.Any(role => userRoles.Contains(role.EnumToString()));
        }
    }
}