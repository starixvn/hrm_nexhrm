namespace Fanex.NexHR.HangFire
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Data.OldConnectionStringProvider;
    using Fanex.Data;
    using Fanex.Logging;
    using Resources;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Dictionary<string, string> connections = ConfigurationManager.ConnectionStrings
                            .Cast<ConnectionStringSettings>()
                            .Where(c => c.Name.Equals("nexhr"))
                            .ToDictionary(v => v.Name, v => v.ConnectionString);

            DbSettingProviderManager
                .StartNewSession()
                .UseConnectionStrings(connections)
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider(
                    resourcePath: "~/App_Data",
                    enableWatching: true)
                .Run();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (!Request.IsLocal)
            {
                Exception ex = Server.GetLastError().GetBaseException();
                string message = string.Format(
                    "{0}{1}{2}{3}{1}{4}{5}{1}{6}{7}{1}{8}{9}",
                    ex.Message,
                    Environment.NewLine,
                    Labels.Source,
                    ex.Source,
                    Labels.Domain,
                    HttpContext.Current.Request.Headers["Host"],
                    Labels.TargetSite,
                    ex.TargetSite,
                    Labels.StackTrace,
                    ex.StackTrace);

                LogService.Error(message);
            }
        }
    }
}