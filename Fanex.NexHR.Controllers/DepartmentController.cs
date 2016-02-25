namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService departmentService;
        private readonly ICommandService commandService;

        public DepartmentController()
            : this(null, null)
        {
        }

        public DepartmentController(
            IDepartmentService departmentService,
            ICommandService commandService)
        {
            this.departmentService = departmentService ?? new DepartmentService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            IList<Department> departments = departmentService.GetAll();

            ViewBag.Departments = departments;
            return View();
        }

        public ActionResult Create()
        {
            var criteria = new GetAllDepartmentCriteria();
            IList<Department> departments = departmentService.GetAll();
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
                };

                CommandResult commandResult = commandService.Insert(criteria);
                ViewBag.Inserted = commandResult.Code == 0;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            IEnumerable<Department> departments = departmentService.GetAll();
            ViewBag.Departments = departments;

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            IEnumerable<Department> departments = departmentService.GetAll();

            Department department = departments.Where(dep => dep.Id == id).SingleOrDefault();
            ViewBag.Departments = departments.Where(dep => dep.Id != id);

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
                DepartmentId = department.Id,
                ManagerId = managerId,
                ManagerName = managerName,
                ParentId = department.Parent == null ? 0 : department.Parent.Id
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

                CommandResult commandResult = commandService.Update(criteria);
                ViewBag.Updated = commandResult.Code == 0;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            IEnumerable<Department> departments = departmentService.GetAll();
            ViewBag.Departments = departments.Where(dep => dep.Id != model.DepartmentId);

            return View(model);
        }

        public JsonResult DeleteDepartment(int id)
        {
            var criteria = new DeleteDepartmentCriteria
            {
                DepartmentId = id,
                ModifiedBy = this.CurrentUser.Id                
            };

            CommandResult commandResult = commandService.Delete(criteria);
            JsonResult result = Json(commandResult, JsonRequestBehavior.AllowGet);
            return result;
        }
    }
}
