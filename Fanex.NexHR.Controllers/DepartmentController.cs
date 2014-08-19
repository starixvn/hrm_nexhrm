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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

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
            IEnumerable<DepartmentModel> departments = _departmentService.GetAllDepartment(criteria);

            ViewBag.Departments = departments;
            return View();
        }

        public ActionResult Create()
        {
            var criteria = new GetAllDepartmentCriteria();
            IEnumerable<DepartmentModel> departments = _departmentService.GetAllDepartment(criteria);
            ViewBag.Departments = departments;

            var model = new DepartmentViewModel();
            return View(model);
        }

        /// <summary>
        /// Create new department.
        /// </summary>
        /// <param name="model">The department view model.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public ActionResult Create(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new InsertDepartmentCriteria
                {
                    Code = model.Code,
                    Name = model.Name,
                    ManagerId = model.ManagerId,
                    ParentId = model.ParentId,
                    CreatedBy = this.CurrentUser.Id,
                    CreatedDate = DateTime.Now
                };

                CommandResult commandResult = _departmentService.InsertDepartment(criteria);
                ViewBag.Inserted = commandResult.Code == 0;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            var getAllDepartmentCriteria = new GetAllDepartmentCriteria();
            IEnumerable<DepartmentModel> departments = _departmentService.GetAllDepartment(getAllDepartmentCriteria);
            ViewBag.Departments = departments;

            return View(model);
        }

        /// <summary>
        /// Edit department.
        /// </summary>
        /// <param name="id">The department Id.</param>
        /// <returns>The view to edit department.</returns>
        public ActionResult Edit(int id)
        {
            var getAllDepartmentCriteria = new GetAllDepartmentCriteria();
            IEnumerable<DepartmentModel> departments = _departmentService.GetAllDepartment(getAllDepartmentCriteria);
            ViewBag.Departments = departments;

            DepartmentModel department = departments.Where(dep => dep.DepartmentId == id).SingleOrDefault();

            string managerName = string.Empty;
            int managerId = 0;

            if (department.Manager != null) 
            {
                Employee manager = department.Manager;
                managerName = manager.NickName + " (" + manager.FullName + ")";
                managerId = manager.Id;
            }

            var model = new DepartmentViewModel
            {
                Code = department.Code,
                Name = department.Name,
                DepartmentId = department.DepartmentId,
                ManagerId = managerId,
                ManagerName = managerName,
                ParentId = department.ParentId
            };

            return View(model);
        }

        /// <summary>
        /// Edit department.
        /// </summary>
        /// <param name="model">The department view model.</param>
        /// <returns>The view after edited department.</returns>
        [HttpPost]
        public ActionResult Edit(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateDepartmentCriteria
                {
                    DepartmentId = model.DepartmentId,
                    Code = model.Code,
                    Name = model.Name,
                    ParentId = model.ParentId,
                    ManagerId = model.ManagerId,
                    ModifiedBy = this.CurrentUser.Id,
                    ModifiedDate = DateTime.Now
                };

                CommandResult commandResult = _departmentService.UpdateDepartment(criteria);
                ViewBag.Updated = commandResult.Code == 0;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            var getAllDepartmentCriteria = new GetAllDepartmentCriteria();
            IEnumerable<DepartmentModel> departments = _departmentService.GetAllDepartment(getAllDepartmentCriteria);
            ViewBag.Departments = departments;

            return View(model);
        }
    }
}
