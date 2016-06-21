// --------------------------------------------------------------------------
// <copyright file="Index.aspx.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.11.6">Created</change>
// </history>

namespace Fanex.NexHR.WebSite
{
    using System;
    using System.Web.Security;

    public partial class Index : System.Web.UI.Page
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