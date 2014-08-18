// --------------------------------------------------------------------------
// <copyright file="DepartmentController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.08.18">Created.</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// The Department controller.
    /// </summary>
    [Authorize]
    public class DepartmentController : BaseController
    {
        private IDepartmentService _departmentService;

        public DepartmentController()
            : this(null)
        {
        }

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService ?? new DepartmentService();
        }

        /// <summary>
        /// Get all departments.
        /// </summary>
        /// <returns>The Action Result.</returns>
        public ActionResult Index()
        {
            var criteria = new GetAllDepartmentCriteria();
            List<DepartmentModel> departments = _departmentService.GetAllDepartment(criteria);

            if (departments != null)
            {
                var treeDepartment = new DepartmentModel();
                treeDepartment.Children.Add(departments.Where(x => x.ParentId == null).SingleOrDefault());
                ViewBag.TreeDepartment = treeDepartment;
            }

            ViewBag.Departments = departments;
            return View();
        }
    }
}
