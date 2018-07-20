using Fanex.Data.OldConnectionStringProvider;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Fanex.NexHR.HangFire.Startup))]

namespace Fanex.NexHR.HangFire
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var oldCs = new OldConnectionStringProvider();
            GlobalConfiguration.Configuration
               .UseSqlServerStorage(oldCs.GetConnectionString("hangfire"));

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            var initEvents = new EventsInitial();
            initEvents.Initialize();
        }
    }
}