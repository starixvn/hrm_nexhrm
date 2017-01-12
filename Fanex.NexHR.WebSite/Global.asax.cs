// --------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.11.6">Created</change>
// </history>

namespace Fanex.NexHR.WebSite
{
    using Fanex.Data.Configuration;
    using Fanex.Logging;
    using Fanex.NexHR.Resources;
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles();
            DbSettingProviderManager.Current.Start();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (!Request.IsLocal)
            {
                Exception ex = Server.GetLastError().GetBaseException();
                string message = ex.Message +
                         Environment.NewLine + Labels.Source + ex.Source +
                        Environment.NewLine + Labels.Domain + HttpContext.Current.Request.Headers["Host"] +
                        Environment.NewLine + Labels.TargetSite + ex.TargetSite +
                        Environment.NewLine + Labels.StackTrace + ex.StackTrace;

                LogService.Error(message);
            }
        }
    }
}