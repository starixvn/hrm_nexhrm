namespace Fanex.NexHR.Models.Attributes
{
    using System;
    using System.Net;
    using Fanex.NexHR.Core;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class HrmAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var employee = context.HttpContext.Session.Get<Employee>("CurrentUser");
            if (employee is null || employee.Id == 0)
            {
                UnauthorizedResponse(context);
            }
        }

        private static void UnauthorizedResponse(AuthorizationFilterContext context)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Result = new RedirectResult("/Auth/Logout");
        }
    }
}