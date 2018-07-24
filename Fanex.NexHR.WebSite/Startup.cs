using Fanex.Data.OldConnectionStringProvider;
using Fanex.NexHR.NexBot;
using Fanex.NexHR.WebSite.App_Start;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Fanex.NexHR.WebSite.Startup))]

namespace Fanex.NexHR.WebSite
{
    public static class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            var oldCs = new OldConnectionStringProvider();
            GlobalConfiguration.Configuration
               .UseSqlServerStorage(oldCs.GetConnectionString("hangfire"));

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
            app.UseHangfireServer();

            var initEvents = new Initial();
            initEvents.Initialize();
        }
    }
}