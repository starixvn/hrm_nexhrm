using Fanex.NexHR.WebSite;
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
    //using System;
    //using System.Web;
    //using System.Web.Http;
    //using System.Web.Mvc;
    //using System.Web.Routing;
    //using Data;
    //using Data.OldConnectionStringProvider;
    //using Logging;
    //using Resources;

    //public class MvcApplication : HttpApplication
    //{
    //    protected void Application_Start()
    //    {
    //        AreaRegistration.RegisterAllAreas();

    //        GlobalConfiguration.Configure(WebApiConfig.Register);
    //        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    //        RouteConfig.RegisterRoutes(RouteTable.Routes);
    //        BundleConfig.RegisterBundles();

    //        DbSettingProviderManager
    //            .StartNewSession()
    //            .Use(new OldConnectionStringProvider())
    //            .UseDefaultDbSettingProvider(
    //                resourcePath: "~/App_Data",
    //                enableWatching: true)
    //            .Run();
    //    }

    //    protected void Application_Error(object sender, EventArgs e)
    //    {
    //        if (!Request.IsLocal)
    //        {
    //            Exception ex = Server.GetLastError().GetBaseException();
    //            string message = string.Format(
    //                "{0}{1}{2}{3}{1}{4}{5}{1}{6}{7}{1}{8}{9}",
    //                ex.Message,
    //                Environment.NewLine,
    //                Labels.Source,
    //                ex.Source,
    //                Labels.Domain,
    //                HttpContextHelper.Current.Request.Headers["Host"],
    //                Labels.TargetSite,
    //                ex.TargetSite,
    //                Labels.StackTrace,
    //                ex.StackTrace);

    //            LogService.Error(message);
    //        }
    //    }
    //}
}