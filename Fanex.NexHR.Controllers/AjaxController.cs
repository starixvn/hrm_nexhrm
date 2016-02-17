// --------------------------------------------------------------------------
// <copyright file="AjaxController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
//   <change who="Josh.Phan" date="2014.03.11">Apply NAP Framework</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class AjaxController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IRequestService _requestService;
        private readonly IHolidayService _holidayService;
        private readonly IMemoService _memoService;
        private readonly ITeamService _teamService;
        private readonly IReportService _reportService;
        private readonly IExtensionNoService _extensionNoService;
        private readonly IDepartmentService _departmentService;

        public AjaxController(
            IEmployeeService employeeService = null,
            IRequestService requestService = null,
            IHolidayService holidayService = null,
            IMemoService memoService = null,
            ITeamService teamService = null,
            IReportService reportService = null,
            IExtensionNoService extensionNoService = null,
            IDepartmentService departmentService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _requestService = requestService ?? new RequestService();
            _holidayService = holidayService ?? new HolidayService();
            _memoService = memoService ?? new MemoService();
            _teamService = teamService ?? new TeamService();
            _reportService = reportService ?? new ReportService();
            _extensionNoService = extensionNoService ?? new ExtensionNoService();
            _departmentService = departmentService ?? new DepartmentService();
        }

        [Authorize(Roles = "AssistantLeader,HR")]
        [HttpPost]
        public JsonResult ProcessRequest(string command, long id)
        {
            CommandResult result = _requestService.Process(command, id, this.CurrentUser.Id);

            if (result.Code == 0)
            {
                return Json(true);
            }

            return Json(result.Message);
        }

        [HttpPost]
        public string GetRequestDetail(RequestEntity request, short requestType, string fromShiftCode, string toShiftCode)
        {
            request.RequestType = new RequestTypeModel { Id = requestType };
            request.FromShift = new Shift { Name = fromShiftCode };
            request.ToShift = new Shift { Name = toShiftCode };

            string requestDetail = RequestService.ToRequestMessage(request);
            return requestDetail;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult AdjustAnnualLeaves(AnnualLeaveBalanceMultiViewModel model)
        {
            if (ModelState.IsValid)
            {
                var adjustAnnualLeavesCriteria = new AdjustAnnualLeavesCriteria
                {
                    EmployeeId = model.EmpIds,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id
                };

                return Json(_employeeService.AdjustAnnualLeaves(adjustAnnualLeavesCriteria));
            }
            else
            {
                return Json(-1);
            }
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult OpenAnnualLeaves()
        {
            var model = new ShowOpenAndCloseAnnualBalance();
            var criteria = new OpenAnnualBalanceForNewYearCriteria()
            {
                Year = model.ToDate.Year,
                CreatedDate = DateTime.Now.Date
            };

            return Json(_employeeService.OpenAnnualLeaves(criteria));
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ResetAnnualLeaves(int minbalance, string descriptionReset)
        {
            if (ModelState.IsValid)
            {
                var model = new ShowOpenAndCloseAnnualBalance();

                var getAnnualLeaveCriteria = new GetAnnualLeaveSummaryReportCriteria
                {
                    EmployeeId = null,
                    GroupId = null,
                    Year = model.FromDate.Year
                };

                IList<LeaveSummary> entityList = _reportService.GetAnnualLeaveBalanceReport(getAnnualLeaveCriteria).Where(m => m.LeaveBalance > minbalance).ToList();

                var resetAnnualLeaves = new ResetAnnualLeavesCriteria
                {
                    Description = descriptionReset,
                    FromDate = new DateTime(model.FromDate.Year, 12, 31),
                    ResetDays = minbalance,
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id,
                    LeaveSummaries = entityList
                };

                return Json(_employeeService.ResetAnnualLeaves(resetAnnualLeaves));
            }
            else
            {
                return Json(-1);
            }
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult AddLeaveDeduction(LeaveDeductionViewModel model)
        {
            var criteria = new InsertLeaveDeductionCriteria
            {
                EmployeeId = model.EmpId,
                Description = model.Description,
                DeductDate = model.DeductDate,
                IsAnnual = model.IsAnnual,
                TotalDays = model.TotalDays,
                CreatedBy = this.CurrentUser.Id
            };

            return Json(_employeeService.AddLeaveDeduction(criteria));
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult EditLeaveDeduction(LeaveDeductionViewModel model)
        {
            var criteria = new UpdateLeaveDeductionCriteria
            {
                LeaveDeductionId = model.LeaveDeductionId,
                Description = model.Description,
                DeductDate = model.DeductDate,
                IsAnnual = model.IsAnnual,
                TotalDays = model.TotalDays,
                ModifiedBy = this.CurrentUser.Id
            };

            return Json(_employeeService.EditLeaveDeduction(criteria));
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult DeleteLeaveDeduction(int leaveDeductionId)
        {
            var criteria = new DeleteLeaveDeductionCriteria
            {
                LeaveDeductionId = leaveDeductionId,
                ModifiedBy = this.CurrentUser.Id
            };

            return Json(_employeeService.DeleteLeaveDeduction(criteria));
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult InsertWorkingCalendar(InsertEmployeeCalendarViewModel model)
        {
            var result = false;
            if (model != null)
            {
                var criteria = new InsertEmployeeCalendarCriteria()
                {
                    EmployeeId = model.EmployeeId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate
                };

                result = _employeeService.InsertWorkingCalendar(criteria);
            }

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteHoliday(int id)
        {
            return _holidayService.Delete(id, this.CurrentUser.Id);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public int AddNewTeam(TeamViewModel model)
        {
            var criteria = new AddNewTeamCriteria()
            {
                Code = model.TeamCode,
                Name = model.TeamName,
                LeaderId = model.LeaderId,
                AssistantLeaderId = model.AssistantLeaderId != null ? model.AssistantLeaderId.Value : default(int),
                CreatedDate = model.CreatedDate,
                CreatedBy = model.CreatedBy
            };

            return _teamService.AddNewTeam(criteria);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public string DeleteTeam(int id)
        {
            var criteria = new DeleteTeamCriteria()
            {
                Id = id,
                ModifiedBy = this.CurrentUser.Id,
                ModifiedDate = DateTime.Now
            };

            CommandResult result = _teamService.DeleteTeam(criteria);

            if (result.Code != 0)
            {
                return result.Message;
            }

            return string.Empty;
        }

        [Authorize(Roles = "HR")]
        public bool UpdateShift(int id, DateTime date, int? shift)
        {
            var updateShiftCriteria = new UpdateShiftCriteria
            {
                EmpId = id,
                Date = date,
                ShiftId = shift,
                ModifiedBy = this.CurrentUser.Id,
                ModifiedDate = DateTime.Now.Date
            };
            bool updated = _employeeService.UpdateShift(updateShiftCriteria);

            return updated;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteExtensionNumber(int extId)
        {
            var criteria = new DeleteExtensionNumberCriteria
            {
                ExtId = extId
            };
            CommandResult result = _extensionNoService.DeleteExtensionNumber(criteria);

            return result.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool AddNewExternalGroup(string groupName)
        {
            var criteria = new AddNewExternalGroupCriteria
            {
                FullName = groupName
            };
            CommandResult result = _extensionNoService.AddNewExternalGroup(criteria);

            return result.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteExternalGroup(int extId)
        {
            var criteria = new DeleteExternalGroupCriteria
            {
                ExtId = extId
            };
            CommandResult result = _extensionNoService.DeleteExternalGroup(criteria);

            return result.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool EditExternalGroup(int extId, string groupName)
        {
            var criteria = new UpdateExternalGroupCriteria
            {
                ExtId = extId,
                FullName = groupName
            };
            CommandResult result = _extensionNoService.UpdateExternalGroup(criteria);

            return result.Code == 0;
        }

        public JsonResult DeleteDepartment(int id)
        {
            var criteria = new DeleteDepartmentCriteria
            {
                DepartmentId = id,
                ModifiedBy = this.CurrentUser.Id,
                ModifiedDate = DateTime.Now
            };

            CommandResult commandResult = _departmentService.DeleteDepartment(criteria);
            JsonResult result = Json(commandResult, JsonRequestBehavior.AllowGet);
            return result;
        }
    }
}