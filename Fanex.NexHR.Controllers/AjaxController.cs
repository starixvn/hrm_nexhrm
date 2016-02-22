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
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.ExtensionNo;
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

        public AjaxController()
            : this(null, null, null, null, null, null, null, null)
        {
        }

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
    }
}