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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Ajax controller.
    /// </summary>
    [Authorize]
    public class AjaxController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IRequestService _requestService;
        private readonly IHolidayService _holidayService;
        private readonly IMemoService _memoService;
        private readonly ITeamService _teamService;
        private readonly ILookupDataService _lookupDataService;
        private readonly IReportService _reportService;
        private readonly IExtensionNoService _extensionNoService;
        private readonly IDepartmentService _departmentService;

        public AjaxController()
            : this(null, null, null, null, null, null, null, null, null)
        {
        }

        public AjaxController(
            IEmployeeService employeeService = null, 
            IRequestService requestService = null, 
            IHolidayService holidayService = null, 
            IMemoService memoService = null, 
            ITeamService teamService = null, 
            ILookupDataService lookupDataService = null,
            IReportService reportService = null,
            IExtensionNoService extensionNoService = null,
            IDepartmentService departmentService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _requestService = requestService ?? new RequestService();
            _holidayService = holidayService ?? new HolidayService();
            _memoService = memoService ?? new MemoService();
            _teamService = teamService ?? new TeamService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _reportService = reportService ?? new ReportService();
            _extensionNoService = extensionNoService ?? new ExtensionNoService();
            _departmentService = departmentService ?? new DepartmentService();
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteEmployee(int id, DateTime resignedDate)
        {
            var criteria = new DeleteEmployeeCriteria()
            {
                Id = id,
                ResignedDate = resignedDate
            };
            bool deleted = _employeeService.Delete(criteria);

            return deleted;
        }

        /// <summary>
        /// Process request..
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="id">The request ID.</param>
        /// <returns>JSON result.</returns>
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

        /// <summary>
        /// Get member by manager.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <param name="groupId">The group ID.</param>
        /// <returns>JSON result.</returns>
        [Authorize(Roles = "AssistantLeader,HR")]
        public JsonResult GetMembersByManager(string keyword, int? groupId = null)
        {
            var role = this.CurrentUser.IsInRoles(Role.HR) ? Role.HR : Role.AssistantLeader;
            var getMemberByManager = new GetMemberByManagerCriteria
            {
                ManagerId = this.CurrentUser.Id,
                Role = (short)role,
                Keyword = keyword,
                GroupId = groupId,
                ExcludedManager = false
            };

            return Json(_employeeService.GetMembersByManager(getMemberByManager).Take(10), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adjust annual leaves.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>JSON result.</returns>
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

        /// <summary>
        /// Add annual leaves.
        /// </summary>
        /// <returns>JSON result.</returns>
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

        /// <summary>
        /// Reset annual leave.
        /// </summary>
        /// <param name="minbalance">The min balance.</param>
        /// <param name="descriptionReset">The description reset.</param>
        /// <returns>JSON result.</returns>
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

                IList<LeaveSummary> entityList = _reportService.GetAnnualLeaveBalanceReport(getAnnualLeaveCriteria);

                entityList = entityList.Where(m => m.LeaveBalance > minbalance).ToList();

                string template = "<Employee Id=\"{0}\" LeaveBalance=\"{1}\"/>";
                StringBuilder strValue = new StringBuilder();
                strValue.Append("<ROOT>");
                foreach (var item in entityList)
                {
                    strValue.Append(string.Format(template, item.Employee.Id, item.LeaveBalance));
                }

                strValue.Append("</ROOT>");
                var resetAnnualLeaves = new ResetAnnualLeavesCriteria
                {
                    EmployeeValue = strValue.ToString(),
                    Description = descriptionReset,
                    FromDate = new DateTime(model.FromDate.Year, 12, 31),
                    ResetDays = minbalance,
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id
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

        /// <summary>
        /// Method to insert working calendars of employee.
        /// </summary>
        /// <param name="model">InsertEmployeeCalendar ViewModel.</param>
        /// <returns>JSON result.</returns>
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
        public bool DeleteMemo(int id)
        {
            var deleteMemoCriteria = new DeleteMemoCriteria() { Id = id };
            return _memoService.Delete(deleteMemoCriteria);
        }

        /// <summary>
        /// Add new Team.
        /// </summary>
        /// <param name="model">Team ViewModel.</param>
        /// <returns>Value to check when new team is added.</returns>
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

        /// <summary>
        /// Delete team.
        /// </summary>
        /// <param name="id">The team ID.</param>
        /// <returns>The error message.</returns>
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

        /// <summary>
        /// Update Shift.
        /// </summary>
        /// <param name="id">Employee id.</param>
        /// <param name="date">Date time.</param>
        /// <param name="shift">Shift's id.</param>
        /// <returns>Boolean to check.</returns>
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
        public bool DeleteJobTitle(int id)
        {
            var deleteLookupDataCriteria = new DeleteLookupDataCriteria() { Id = id };
            bool deleted = _lookupDataService.Delete(deleteLookupDataCriteria);

            return deleted;
        }

        /// <summary>
        /// Insert Job Title.
        /// </summary>
        /// <param name="name">Job's name.</param>
        /// <returns>Boolean to check.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool InsertJobTitle(string name)
        {
            bool inserted = false;
            var insertLookupDataCriteria = new InsertLookupDataCriteria()
            {
                LookupCode = LookupCodeName.JobTitle,
                Name = name
            };
            inserted = _lookupDataService.Insert(insertLookupDataCriteria);

            return inserted;
        }

        /// <summary>
        /// Update Job Title.
        /// </summary>
        /// <param name="id">Current id.</param>
        /// <param name="name">New Job name.</param>
        /// <returns>Boolean to check.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool UpdatetJobTitle(int id, string name)
        {
            bool updated = false;
            var updateLookupDataCriteria = new UpdateLookupDataCriteria()
            {
                Id = id,
                Name = name
            };
            updated = _lookupDataService.Update(updateLookupDataCriteria);

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

        /// <summary>
        /// Delete department.
        /// </summary>
        /// <param name="id">The department Id.</param>
        /// <returns>The JSON result.</returns>
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
