// --------------------------------------------------------------------------
// <copyright file="AuthController.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed.")]
    public class AuthController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public AuthController()
        {
            _employeeService = new EmployeeService();
        }

        public AuthController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
    }
}
