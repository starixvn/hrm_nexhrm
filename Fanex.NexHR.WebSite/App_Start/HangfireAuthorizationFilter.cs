namespace Fanex.NexHR.WebSite.App_Start
{
    using Controllers;
    using Hangfire.Dashboard;
    using Microsoft.Owin;

    public class HangfireAuthorizationFilter : BaseController, IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var owinContext = new OwinContext(context.GetOwinEnvironment());

            return owinContext.Authentication.User.Identity.IsAuthenticated;
        }
    }
}