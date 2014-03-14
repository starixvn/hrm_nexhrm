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
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Ajax controller.
    /// </summary>
    [Authorize]
    public class AjaxController : BaseController
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRequestService _requestService;
        private readonly IHolidayService _holidayService;
        private readonly IMemoService _memoService;
        private readonly ITeamService _teamService;
        private readonly IEmployeeService _employeeService;
        private readonly ILookupDataService _lookupDataService;
        private readonly IReportRepository _reportRepository;
        private readonly IExtensionNoService _extensionNoService;

        public AjaxController()
        {
            _employeeRepository = new EmployeeRepository();
            _requestService = new RequestService();
            _holidayService = new HolidayService();
            _memoService = new MemoService();
            _teamService = new TeamService();
            _employeeService = new EmployeeService();
            _lookupDataService = new LookupDataService();
            _reportRepository = new ReportRepository();
            _extensionNoService = new ExtensionNoService();
        }

        public AjaxController(
            IEmployeeRepository employeeRepository, 
            IRequestService requestService, 
            IHolidayService holidayService, 
            IMemoService memoService, 
            ITeamService teamService, 
            ILookupDataService lookupDataService, 
            IReportRepository reportRepository,
            IExtensionNoService extensionNoService)
        {
            _employeeRepository = employeeRepository;
            _requestService = requestService;
            _holidayService = holidayService;
            _memoService = memoService;
            _teamService = teamService;
            _lookupDataService = lookupDataService;
            _reportRepository = reportRepository ?? new ReportRepository();
            _extensionNoService = extensionNoService;
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
            bool deleted = _employeeRepository.Delete(criteria);

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
            string errorMessage;
            var result = _requestService.Process(command, id, this.CurrentUser.Id, out errorMessage);
            return result ? Json(true) : Json(errorMessage);
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
            var role = this.CurrentUser.IsInRoles(RolesEnum.HR) ? RolesEnum.HR : RolesEnum.AssistantLeader;
            var getMemberByManager = new GetMemberByManagerCriteria
            {
                ManagerId = this.CurrentUser.Id,
                Role = (short)role,
                Keyword = keyword,
                GroupId = groupId,
                ExcludedManager = false
            };

            return Json(
                _employeeRepository.GetMembersByManager(getMemberByManager).Take(10),
                    JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adjust annual leaves.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>JSON result.</returns>
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

                return Json(_employeeRepository.AdjustAnnualLeaves(adjustAnnualLeavesCriteria));
            }
            else
            {
                return Json(-1);
            }
        }

        /// <summary>
        /// Reset annual leave.
        /// </summary>
        /// <param name="minbalance">The min balance.</param>
        /// <param name="descriptionReset">The description reset.</param>
        /// <returns>JSON result.</returns>
        [HttpPost]
        public JsonResult ResetAnnualLeaves(int minbalance, string descriptionReset)
        {
            if (ModelState.IsValid)
            {
                int lastYear = DateTime.Now.AddYears(-1).Year;
                DateTime date = new DateTime(lastYear, 12, 31);

                var getAnnualLeaveCriteria = new GetAnnualLeaveSummaryReportCriteria
                {
                    EmployeeId = null,
                    GroupId = null,
                    Year = lastYear
                };

                IList<LeaveSummary> entityList = _reportRepository.GetAnnualLeaveBalanceReport(getAnnualLeaveCriteria);

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
                    FromDate = date,
                    ResetDays = minbalance,
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id
                };

                return Json(_employeeRepository.ResetAnnualLeaves(resetAnnualLeaves));
            }
            else
            {
                return Json(-1);
            }
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
        /// <returns>The return string.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public string DeleteTeam(int id)
        {
            int errCode;
            string errMsg;
            var criteria = new DeleteTeamCriteria()
            {
                Id = id,
                ModifiedBy = this.CurrentUser.Id,
                ModifiedDate = DateTime.Now
            };

            _teamService.DeleteTeam(criteria, out errCode, out errMsg);

            if (errCode != 0)
            {
                return errMsg;
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
        public bool UpdateShift(int id, DateTime date, int? shift)
        {
            var updateShiftCriteria = new UpdateShiftCriteria
            {
                EmpId = id,
                Date = date,
                ShiftID = shift,
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
                ExtID = extId
            };
            return _extensionNoService.DeleteExtensionNumber(criteria);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool AddNewExternalGroup(string groupName)
        {
            var criteria = new AddNewExternalGroupCriteria
            {
                FullName = groupName
            };
            return _extensionNoService.AddNewExternalGroup(criteria);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteExternalGroup(int extId)
        {
            var criteria = new DeleteExternalGroupCriteria
            {
                ExtID = extId
            };
            return _extensionNoService.DeleteExternalGroup(criteria);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool EditExternalGroup(int extId, string groupName)
        {
            var criteria = new UpdateExternalGroupCriteria
            {
                ExtID = extId,
                FullName = groupName
            };
            return _extensionNoService.UpdateExternalGroup(criteria);
        }
    }
}
