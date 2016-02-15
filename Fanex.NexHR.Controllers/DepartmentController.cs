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
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService = null)
        {
            _departmentService = departmentService ?? new DepartmentService();
        }

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
                    CreatedBy = CurrentUser.Id,
                    CreatedDate = DateTime.Now
                };

                CommandResult commandResult = _departmentService.InsertDepartment(criteria);
                ViewBag.Inserted = commandResult.Code == 0;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            var getAllDepartmentCriteria = new GetAllDepartmentCriteria();
            IEnumerable<DepartmentModel> departments = _departmentService.GetAllDepartment(getAllDepartmentCriteria);
            ViewBag.Departments = departments;

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var getAllDepartmentCriteria = new GetAllDepartmentCriteria();
            IEnumerable<DepartmentModel> departments = _departmentService.GetAllDepartment(getAllDepartmentCriteria);

            DepartmentModel department = departments.Where(dep => dep.DepartmentId == id).SingleOrDefault();
            ViewBag.Departments = departments.Where(dep => dep.DepartmentId != id);

            string managerName = string.Empty;
            int managerId = 0;

            if (department.Manager != null) 
            {
                Employee manager = department.Manager;
                managerName = manager.DisplayName;
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
            ViewBag.Departments = departments.Where(dep => dep.DepartmentId != model.DepartmentId);

            return View(model);
        }
    }
}
