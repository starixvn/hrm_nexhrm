namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
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
                Teams = teamService.GetTeam(),
                StatusId = (byte)RequestStatuse.Pending
            };
            return View(model);
        }

        public ActionResult GetEmployeeWorkFromHomeGrid(EmployeeWorkFromHomeFilterViewModel model)
        {
            int? statusNullable = null;
            var criteria = new GetEmployeeWorkFromHomeCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                EmpId = model.EmployeeId,
                StatusId = model.StatusId == 0 ? statusNullable : model.StatusId
            };
            var employees = employeeWorkFromHomeService.GetAll(criteria);
            foreach(var employee in employees)
            {
                employee.Status = GetStatusName(employee.StatusId);
            }
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

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ProcessRequest(string command, int id)
        {
            var criteria = new EmployeeWorkFromHomeApproveAndRejectRequestCriteria
            {
                WorkFromHomeRequestId = id,
                StatusId = (byte)(command == Labels.Approve ? 2 : 3),
                ProcessedBy = CurrentUser.Id,
                ProcessedDate = DateTime.Now
            };
            var result = employeeWorkFromHomeService.ProcessApproveAndReject(criteria);
            return Json(result);
        }


        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ProcessDisapproveRequest(int id)
        {
            var criteria = new EmployeeWorkFromHomeDisApproveRequestCriteria
            {
                WorkFromHomeRequestId = id,
                ProcessedBy = CurrentUser.Id,
                ProcessedDate = DateTime.Now
            };
            var result = employeeWorkFromHomeService.ProcessDisapprove(criteria);
            return Json(result);
        }

        private static string GetStatusName(byte Id)
        {
            const int secondCase = 2;
            const int threeCase = 3;

            switch (Id)
            {
                case 1:
                    return Labels.Pending;
                case secondCase:
                    return Labels.Approved;
                case threeCase:
                    return Labels.Rejected;
                default:
                    return string.Empty;
            }
        }
    }
}
