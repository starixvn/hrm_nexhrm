namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.EmployeeWorkFromHome;

    public class EmployeeWorkFromHomeController : BaseController
    {
        private readonly IEmployeeWorkFromHomeService employeeWorkFromHomeService;
        private readonly ITeamService teamService;

        public EmployeeWorkFromHomeController()
            : this(null, null)
        {
        }

        public EmployeeWorkFromHomeController(
            IEmployeeWorkFromHomeService employeeWorkFromHomeService,
            ITeamService teamService)
        {
            this.employeeWorkFromHomeService = employeeWorkFromHomeService ?? new EmployeeWorkFromHomeService();
            this.teamService = teamService ?? new TeamService();
        }

        public ActionResult Index()
        {
            var model = new EmployeeWorkFromHomeFilterViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                Teams = teamService.GetTeam()
            };
            return View(model);
        }

        public ActionResult GetEmployeeWorkFromHomeGrid(DateTime fromDate, DateTime toDate, int? teamId = null, int? employeeId = null)
        {
            var criteria = new GetEmployeeWorkFromHomeCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                GroupId = teamId,
                EmpId = employeeId
            };
            var employees = employeeWorkFromHomeService.GetAll(criteria);
            return PartialView("_EmployeeWorkFromHomeGrid", employees);
            
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            var model = new EmployeeWorkFromHomeViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now
            };
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Create(EmployeeWorkFromHomeViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertEmployeeWorkFromHomeCriteria
                {
                    EmpId = model.EmpId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CreatedBy = CurrentUser.Id,
                    Remark = model.Remark
                };

                var result = employeeWorkFromHomeService.Insert(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Requested = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            var employeeWorkFromHome = employeeWorkFromHomeService.GetById(id);
            var model = new EmployeeWorkFromHomeViewModel
            {
                EmployeeWorkFromHomeId = employeeWorkFromHome.EmployeeWorkFromHomeId,
                EmpId = employeeWorkFromHome.EmpId,
                Employee = string.Format("{0} ({1})", employeeWorkFromHome.Nickname, employeeWorkFromHome.Fullname),
                FromDate = employeeWorkFromHome.FromDate,
                ToDate = employeeWorkFromHome.ToDate,
                Remark = employeeWorkFromHome.Remark
            };
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Edit(EmployeeWorkFromHomeViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateEmployeeWorkFromHomeCriteria
                {
                    EmployeeWorkFromHomeId = model.EmployeeWorkFromHomeId,
                    EmpId = model.EmpId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    Remark = model.Remark
                };

                var result = employeeWorkFromHomeService.Update(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Requested = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Delete(int id)
        {
            var criteria = new DeleteEmployeeWorkFromHomeCriteria { EmployeeWorkFromHomeId = id };
            var result = employeeWorkFromHomeService.Delete(criteria);

            return Json(result);
        }
    }
}
