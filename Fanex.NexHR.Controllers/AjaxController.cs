// --------------------------------------------------------------------------
// <copyright file="AjaxController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
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
        }

        public AjaxController(IEmployeeRepository employeeRepository, IRequestService requestService, IHolidayService holidayService, IMemoService memoService, ITeamService teamService, ILookupDataService lookupDataService, IReportRepository reportRepository)
        {
            _employeeRepository = employeeRepository;
            _requestService = requestService;
            _holidayService = holidayService;
            _memoService = memoService;
            _teamService = teamService;
            _lookupDataService = lookupDataService;
            _reportRepository = reportRepository ?? new ReportRepository();
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteEmployee(int id, DateTime resignedDate)
        {
            return _employeeRepository.Delete(id, resignedDate);
        }

        [Authorize(Roles = "AssistantLeader,HR")]
        [HttpPost]
        public JsonResult ProcessRequest(string command, long id)
        {
            string errorMessage;
            var result = _requestService.Process(command, id, this.CurrentUser.Id, out errorMessage);
            return result ? Json(true) : Json(errorMessage);
        }

        [Authorize(Roles = "AssistantLeader,HR")]
        public JsonResult GetMembersByManager(string keyword, int? groupId = null)
        {
            var role = this.CurrentUser.IsInRoles(RolesEnum.HR) ? RolesEnum.HR : RolesEnum.AssistantLeader;

            return Json(
                _employeeRepository.GetMembersByManager(
                    this.CurrentUser.Id, 
                    (short)role,
                    keyword, 
                    groupId).Take(10),
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AdjustAnnualLeaves(AnnualLeaveBalanceMultiViewModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(_employeeRepository.AdjustAnnualLeaves(
                    model.EmpIds, 
                    model.Description, 
                    model.AffectedFrom,
                    model.DeductDays, 
                    model.CreditDays, 
                    DateTime.Now, 
                    this.CurrentUser.Id));
            }
            else
            {
                return Json(-1);
            }
        }

        [HttpPost]
        public JsonResult ResetAnnualLeaves(int minbalance, string descriptionReset)
        {
            if (ModelState.IsValid)
            {
                int lastYear = DateTime.Now.AddYears(-1).Year;
                DateTime date = new DateTime(lastYear, 12, 31);

                IList<LeaveSummary> entityList = _reportRepository.GetAnnualLeaveBalanceReport(
                    lastYear,
                    empId: null,
                    groupId: null);

                entityList = entityList.Where(m => m.LeaveBalance > minbalance).ToList();

                string template = "<Employee Id=\"{0}\" LeaveBalance=\"{1}\"/>";
                StringBuilder strValue = new StringBuilder();
                strValue.Append("<ROOT>");
                foreach (var item in entityList)
                {
                    strValue.Append(string.Format(template, item.Employee.Id, item.LeaveBalance));
                }

                strValue.Append("</ROOT>");
                return Json(_employeeRepository.ResetAnnualLeaves(
                    strValue.ToString(), 
                    descriptionReset, 
                    date,
                    minbalance, 
                    DateTime.Now, 
                    this.CurrentUser.Id));
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
            return _memoService.Delete(id);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public int AddNewTeam(TeamViewModel model)
        {
            return _teamService.AddNewTeam(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public string DeleteTeam(int id)
        {
            int errCode;
            string errMsg;
            var result = _teamService.DeleteTeam(id, DateTime.Now, this.CurrentUser.Id, out errCode, out errMsg);

            if (errCode != 0)
            {
                return errMsg;
            }

            return string.Empty;
        }

        public bool UpdateShift(int id, DateTime date, int? shift)
        {
            return _employeeService.UpdateShift(id, date, shift, this.CurrentUser.Id, DateTime.Now);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteJobTitle(int id)
        {
            return _lookupDataService.Delete(id);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool InsertJobTitle(string name)
        {
            return _lookupDataService.Insert(name, LookupCodeName.JobTitle);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool UpdatetJobTitle(int id, string name)
        {
            return _lookupDataService.Update(id, name);
        }
    }
}
