namespace Fanex.NexHR.Controllers
{
    using Criteria.Department;
    using Models;
    using Services;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using ViewModels;

    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;
        private readonly ICommandService _commandService;

        public DepartmentController()
            : this(null, null)
        {
        }

        public DepartmentController(
            IDepartmentService departmentService,
            ICommandService commandService)
        {
            _departmentService = departmentService ?? new DepartmentService();
            _commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            IList<Department> departments = _departmentService.GetAll();

            ViewBag.Departments = departments;
            return View();
        }

        public ActionResult Create()
        {
            IList<Department> departments = _departmentService.GetAll();
            ViewBag.Departments = departments;

            var model = new DepartmentViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DepartmentViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertDepartmentCriteria
                {
                    Code = model.Code,
                    Name = model.Name,
                    ManagerId = model.ManagerId,
                    ParentId = model.ParentId,
                    CreatedBy = CurrentUser.Id
                };

                CommandResult commandResult = _commandService.Insert(criteria);
                if (commandResult.IsSuccessful)
                {
                    ViewBag.Inserted = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            IEnumerable<Department> departments = _departmentService.GetAll();
            ViewBag.Departments = departments;

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            IEnumerable<Department> departments = _departmentService.GetAll();

            Department department = departments.SingleOrDefault(dep => dep.Id == id);
            ViewBag.Departments = departments.Where(dep => dep.Id != id);

            string managerName = string.Empty;
            int managerId = 0;

            if (department?.Manager != null) 
            {
                Employee manager = department.Manager;
                managerName = manager.DisplayName;
                managerId = manager.Id;
            }

            if (department != null)
            {
                var model = new DepartmentViewModel
                {
                    Code = department.Code,
                    Name = department.Name,
                    DepartmentId = department.Id,
                    ManagerId = managerId,
                    ManagerName = managerName,
                    ParentId = department.Parent?.Id ?? 0
                };

                return View(model);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Edit(DepartmentViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateDepartmentCriteria
                {
                    DepartmentId = model.DepartmentId,
                    Code = model.Code,
                    Name = model.Name,
                    ParentId = model.ParentId,
                    ManagerId = model.ManagerId,
                    ModifiedBy = CurrentUser.Id
                };

                CommandResult commandResult = _commandService.Update(criteria);

                if (commandResult.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            IEnumerable<Department> departments = _departmentService.GetAll();
            ViewBag.Departments = departments.Where(dep => model != null && dep.Id != model.DepartmentId);

            return View(model);
        }

        public JsonResult DeleteDepartment(int id)
        {
            var criteria = new DeleteDepartmentCriteria
            {
                DepartmentId = id,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult commandResult = _commandService.Delete(criteria);
            return Json(commandResult);
        }
    }
}
