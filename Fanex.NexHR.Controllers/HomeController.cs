// --------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed.")]
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController()
            : this(null)
        { 
        }

        public HomeController(IEmployeeRepository employeeRepository = null)
        {
            _employeeRepository = employeeRepository ?? new EmployeeRepository();
        }

        public ActionResult Index()
        {
            ViewBag.Employee = this.CurrentUser;
            return RedirectToAction("myspace", "employee");
        }

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult AccountSuspended()
        {
            return View();
        }
    }
}
