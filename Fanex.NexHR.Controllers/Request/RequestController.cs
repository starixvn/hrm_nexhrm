// --------------------------------------------------------------------------
// <copyright file="RequestController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed.")]
    [Authorize]
    public class RequestController : BaseController
    {
        #region private services
        protected readonly IEmployeeService _employeeService;
        protected readonly IShiftService _shiftService;
        protected readonly ILeaveTypeService _leaveTypeService;
        protected readonly IRequestService _requestService;
        protected readonly ITeamService _teamService;
        #endregion

        #region Ctor
        public RequestController()
            : this(null, null, null, null)
        {
        }

        public RequestController(
            IEmployeeService employeeService = null,
            IShiftService shiftService = null,
            ILeaveTypeService leaveTypeService = null,
            IRequestService requestService = null,
            ITeamService teamService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _shiftService = shiftService ?? new ShiftService();
            _leaveTypeService = leaveTypeService ?? new LeaveTypeService();
            _requestService = requestService ?? new RequestService();
            _teamService = teamService ?? new TeamService();
        }
        #endregion

        public PartialViewResult CriteriaPanel(RequestViewModel request)
        {
            return PartialView(request);
        }

        #region Member's Action

        [Authorize(Roles = "AssistantLeader")]
        public ActionResult Member(RequestViewModel request)
        {
            if (request.TeamId == -1)
            {
                request = new RequestViewModel 
                {
                    /*get data from 7 days ago*/

                    DateOption = 0,
                    ToDate = DateTime.Now,
                    FromDate = request.ToDate.AddDays(-7),
                    RequestType = 5,
                    TeamId = null,
                };
            }

            if (request.TeamId == -2)
            {
                request.TeamId = null;
            }

            ViewBag.IsAllowChangeStatus = true;
            request.TeamCollection = _teamService.GetTeamManagedByEmp(this.CurrentUser.Id);
            ViewBag.Status = request.StatusId;

            switch (request.RequestType)
            {
                case 1:
                    GetMemberLeaveRequest(request);
                    break;
                case 2:
                    GetMemberChangeShift(request);
                    break;
                case 3:
                    GetMemberLateInEarlyOut(request);
                    break;
                case 4:
                    GetMemberCheckInOut(request);
                    break;
                case 5:
                    GetAllRequest(request, (short)RolesEnum.AssistantLeader);
                    break;
            }

            ViewBag.Leader = true;
            ViewBag.Options = request;
            return View(request);
        }

        #endregion

        #region Member's requests
        [NonAction]
        public void GetMemberLeaveRequest(RequestViewModel request)
        {
            IList<Leave> entityList = _employeeService.GetLeaveRequest(
                   (short)RolesEnum.AssistantLeader,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: this.CurrentUser.Id);

            ViewBag.EntityList = entityList;
        }

        [NonAction]
        public void GetMemberChangeShift(RequestViewModel request)
        {
            IList<ChangingShift> entityList = _employeeService.GetChangeShiftRequest(
                   (short)RolesEnum.AssistantLeader,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: this.CurrentUser.Id);

            ViewBag.EntityList = entityList;
        }

        [NonAction]
        public void GetMemberLateInEarlyOut(RequestViewModel request)
        {
            IList<LateInEarlyOut> entityList = _employeeService.GetLateInEarlyOutRequest(
                   (short)RolesEnum.AssistantLeader,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: this.CurrentUser.Id);

            ViewBag.EntityList = entityList;
        }

        [NonAction]
        public void GetMemberCheckInOut(RequestViewModel request)
        {
            IList<CheckInOut> entityList = _employeeService.GetCheckInOutRequest(
                   (short)RolesEnum.AssistantLeader,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: this.CurrentUser.Id);

            ViewBag.EntityList = entityList;
        }
        #endregion

        #region Request History
        public ActionResult HRViewHistory(long id, int type)
        {
            switch (type)
            {
                case 1:
                    HRGetCheckInOutRequestHistory(id, type);
                    break;
                case 2:
                    HRGetLateInEarlyOutRequestHistory(id, type);
                    break;
                case 3:
                    HRGetChangeShiftRequestHistory(id, type);
                    break;
                case 4:
                    HRGetLeaveRequestHistory(id, type);
                    break;
            }

            ViewBag.requestId = id;
            ViewBag.requestType = type;

            return View();
        }

        public void HRGetLeaveRequestHistory(long requestId, int requestType)
        {
            IList<Leave> entityList = _employeeService.GetLeaveRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        public void HRGetChangeShiftRequestHistory(long requestId, int requestType)
        {
            IList<ChangingShift> entityList = _employeeService.GetChangeShiftRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        public void HRGetLateInEarlyOutRequestHistory(long requestId, int requestType)
        {
            IList<LateInEarlyOut> entityList = _employeeService.GetLateInEarlyOutRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        public void HRGetCheckInOutRequestHistory(long requestId, int requestType)
        {
            IList<CheckInOut> entityList = _employeeService.GetCheckInOutRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        #endregion

        public ActionResult DailyView(RequestViewModel request)
        {
            // Default view all request type
            if (request.TeamId == -1)
            {
                request = new RequestViewModel
                {
                    StatusId = 0,
                    RequestType = 5,
                    TeamId = null
                };
            }

            // Get request model from Request monitoring view.
            if (request.TeamId == -2)
            {
                request.StatusId = 0;
                request.TeamId = null;
            }

            ViewBag.IsAllowChangeStatus = false;
            request.TeamCollection = _teamService.GetAll();

            // request.StatusId = 0;
            ViewBag.Status = request.StatusId;

            switch (request.RequestType)
            {
                case 1:
                    HRGetLeaveRequestByDate(request);
                    break;
                case 2:
                    HRGetChangeShiftRequestByDate(request);
                    break;
                case 3:
                    HRGetLateInEarlyOutRequestByDate(request);
                    break;
                case 4:
                    HRGetCheckInOutRequestByDate(request);
                    break;
                case 5:
                    GetAllRequestByDate(request, (short)RolesEnum.HR);
                    break;
            }

            ViewBag.Options = request;
            return View("DailyRequestsView", request);
        }

        public ActionResult EmployeeView(RequestViewModel request)
        {
            // Default view all request type
            if (request.TeamId == -1)
            {
                request = new RequestViewModel
                {
                    StatusId = 0,
                    RequestType = 5,
                    TeamId = null
                };
            }

            // Get request model from Request monitoring view.
            if (request.TeamId == -2)
            {
                request.StatusId = 0;
                request.TeamId = null;
            }

            ViewBag.IsAllowChangeStatus = false;
            request.TeamCollection = _teamService.GetAll();

            // request.StatusId = 0;
            ViewBag.Status = request.StatusId;

            switch (request.RequestType)
            {
                case 1:
                    HRGetLeaveRequest(request);
                    break;
                case 2:
                    HRGetChangeShiftRequest(request);
                    break;
                case 3:
                    HRGetLateInEarlyOutRequest(request);
                    break;
                case 4:
                    HRGetCheckInOutRequest(request);
                    break;
                case 5:
                    GetAllRequest(request, (short)RolesEnum.HR);
                    break;
            }

            ViewBag.Options = request;
            return View("HRView", request);
        }
        #region HR's Action

        [Authorize(Roles = "HR")]
        public ActionResult HRView(RequestViewModel request)
        {
            // Default view all request type
            if (request.TeamId == -1)
            {
                request = new RequestViewModel 
                {
                    StatusId = 0,
                    RequestType = 5,
                    TeamId = null,
                };
            }

            // Get request model from Request monitoring view.
            if (request.TeamId == -2)
            {
                request.StatusId = 0;
                request.TeamId = null;
            }

            ViewBag.IsAllowChangeStatus = true;
            request.TeamCollection = _teamService.GetAll();

           // request.StatusId = 0;
            ViewBag.Status = request.StatusId;

            switch (request.RequestType)
            {
                case 1:
                    HRGetLeaveRequest(request);
                    break;
                case 2:
                    HRGetChangeShiftRequest(request);
                    break;
                case 3:
                    HRGetLateInEarlyOutRequest(request);
                    break;
                case 4:
                    HRGetCheckInOutRequest(request);
                    break;
                case 5:
                    GetAllRequest(request, (short)RolesEnum.HR);
                    break;
            }

            ViewBag.Options = request;
            return View(request);
        }

        #endregion

        #region Daily Requests

        public void HRGetLeaveRequestByDate(RequestViewModel request)
        {
            IList<Leave> entityList = _employeeService.GetLeaveRequestByDate(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        public void HRGetChangeShiftRequestByDate(RequestViewModel request)
        {
            IList<ChangingShift> entityList = _employeeService.GetChangeShiftRequestByDate(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        public void HRGetLateInEarlyOutRequestByDate(RequestViewModel request)
        {
            IList<LateInEarlyOut> entityList = _employeeService.GetLateInEarlyOutRequestByDate(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        public void HRGetCheckInOutRequestByDate(RequestViewModel request)
        {
            IList<CheckInOut> entityList = _employeeService.GetCheckInOutRequestByDate(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        public void GetAllRequestByDate(RequestViewModel request, short role)
        {
            IList<RequestEntity> entityList = _employeeService.GetAllRequestByDate(
                  role,
                  request.FromDate,
                  request.ToDate,
                  employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                  groupId: request.TeamId,
                  status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                  logInId: role == (short)RolesEnum.AssistantLeader ? new int?(this.CurrentUser.Id) : null);

            ViewBag.EntityList = entityList;
        }

        #endregion

        #region HRView's requests

        public void HRGetLeaveRequest(RequestViewModel request)
        {
            IList<Leave> entityList = _employeeService.GetLeaveRequest(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        public void HRGetChangeShiftRequest(RequestViewModel request)
        {
            IList<ChangingShift> entityList = _employeeService.GetChangeShiftRequest(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        public void HRGetLateInEarlyOutRequest(RequestViewModel request)
        {
            IList<LateInEarlyOut> entityList = _employeeService.GetLateInEarlyOutRequest(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        public void HRGetCheckInOutRequest(RequestViewModel request)
        {
            IList<CheckInOut> entityList = _employeeService.GetCheckInOutRequest(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        #endregion

        public void GetAllRequest(RequestViewModel request, short role)
        {
            IList<RequestEntity> entityList = _employeeService.GetAllRequest(
                  role,
                  request.FromDate,
                  request.ToDate,
                  employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                  groupId: request.TeamId,
                  status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                  logInId: role == (short)RolesEnum.AssistantLeader ? new int?(this.CurrentUser.Id) : null);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// Check if user can and has permission to edit a request.
        /// </summary>
        /// <param name="status">Status of request.</param>
        /// <param name="employeeId">Which employee request belong to.</param>
        /// <param name="creatorId">The person who creates this request.</param>
        /// <returns>Return true if current user can edit this request.</returns>
        protected bool Editable(byte status, int employeeId, int creatorId)
        {
            if (status != (byte)RequestStatuses.Pending ||
                (employeeId != this.CurrentUser.Id && creatorId != this.CurrentUser.Id))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
