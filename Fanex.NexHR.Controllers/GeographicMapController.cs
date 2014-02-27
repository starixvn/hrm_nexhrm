// -----------------------------------------------------------------------
// <copyright file="GeographicMapController.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.Core.Enums;

    /// <summary>
    /// Geographic Map Controller.
    /// </summary>
    [Authorize]
    public class GeographicMapController : BaseController
    {
        private readonly IEmployeeService _employeeService;

        public GeographicMapController(IEmployeeService employeeService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
        }

        public GeographicMapController()
            : this(null)
        {
        }

        public ActionResult Index()
        {
            bool roleHR = false;
            var user = this.CurrentUser;
            if (user.IsInRoles(RolesEnum.HR))
            {
                roleHR = true;
            }
            ViewBag.NickName = user.NickName;
            ViewBag.RoleHR = roleHR;

            return View();
        }
    }
}
