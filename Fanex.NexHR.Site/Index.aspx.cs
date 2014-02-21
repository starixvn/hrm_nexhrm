// --------------------------------------------------------------------------
// <copyright file="Index.aspx.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Site
{
    using System;
    using System.Web.Security;
    using System.Web.UI;

    /// <summary>
    /// Index page.
    /// </summary>
    public partial class Index : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            else
            {
                Response.Redirect("~/employee/myspace");
            }
        }
    }
}