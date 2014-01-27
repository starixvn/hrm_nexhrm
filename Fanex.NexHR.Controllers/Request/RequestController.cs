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

    /// <summary>
    /// Request controller.
    /// </summary>
    [Authorize]
    public class RequestController : BaseController
    {
        #region private services
        protected readonly IEmployeeService EmployeeService;
        protected readonly IShiftService ShiftService;
        protected readonly ILeaveTypeService LeaveTypeService;
        protected readonly IRequestService RequestService;
        protected readonly ITeamService TeamService;
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
            EmployeeService = employeeService ?? new EmployeeService();
            ShiftService = shiftService ?? new ShiftService();
            LeaveTypeService = leaveTypeService ?? new LeaveTypeService();
            RequestService = requestService ?? new RequestService();
            TeamService = teamService ?? new TeamService();
        }
        #endregion

        public PartialViewResult CriteriaPanel(RequestViewModel request)
        {
            return PartialView(request);
        }

        #region Member's Action

        /// <summary>
        /// View member request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Action result.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
            request.TeamCollection = TeamService.GetTeamManagedByEmp(this.CurrentUser.Id);
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

        /// <summary>
        /// Get member leave request.
        /// </summary>
        /// <param name="request">The request.</param>
        [NonAction]
        public void GetMemberLeaveRequest(RequestViewModel request)
        {
            IList<Leave> entityList = EmployeeService.GetLeaveRequest(
                   (short)RolesEnum.AssistantLeader,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: this.CurrentUser.Id);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// Get member change shift request.
        /// </summary>
        /// <param name="request">The request.</param>
        [NonAction]
        public void GetMemberChangeShift(RequestViewModel request)
        {
            IList<ChangingShift> entityList = EmployeeService.GetChangeShiftRequest(
                   (short)RolesEnum.AssistantLeader,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: this.CurrentUser.Id);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// Get member late in / early out request.
        /// </summary>
        /// <param name="request">The request.</param>
        [NonAction]
        public void GetMemberLateInEarlyOut(RequestViewModel request)
        {
            IList<LateInEarlyOut> entityList = EmployeeService.GetLateInEarlyOutRequest(
                   (short)RolesEnum.AssistantLeader,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: this.CurrentUser.Id);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// Get member check in/out request.
        /// </summary>
        /// <param name="request">The request.</param>
        [NonAction]
        public void GetMemberCheckInOut(RequestViewModel request)
        {
            IList<CheckInOut> entityList = EmployeeService.GetCheckInOutRequest(
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

        /// <summary>
        /// HR view request history.
        /// </summary>
        /// <param name="id">The request ID.</param>
        /// <param name="type">The request type.</param>
        /// <returns>Action result.</returns>
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

        /// <summary>
        /// HR get leave request history.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <param name="requestType">The request type.</param>
        public void HRGetLeaveRequestHistory(long requestId, int requestType)
        {
            IList<Leave> entityList = EmployeeService.GetLeaveRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get change shift request history.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <param name="requestType">The request type.</param>
        public void HRGetChangeShiftRequestHistory(long requestId, int requestType)
        {
            IList<ChangingShift> entityList = EmployeeService.GetChangeShiftRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get late in / early out request history..
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <param name="requestType">The request type.</param>
        public void HRGetLateInEarlyOutRequestHistory(long requestId, int requestType)
        {
            IList<LateInEarlyOut> entityList = EmployeeService.GetLateInEarlyOutRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get check in/out request history..
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <param name="requestType">The request type.</param>
        public void HRGetCheckInOutRequestHistory(long requestId, int requestType)
        {
            IList<CheckInOut> entityList = EmployeeService.GetCheckInOutRequestHistory(requestId, requestType);
            ViewBag.EntityList = entityList;
        }

        #endregion

        /// <summary>
        /// Daily view request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Action result.</returns>
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
            request.TeamCollection = TeamService.GetAll();

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

        /// <summary>
        /// Employee view request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Action result.</returns>
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
            request.TeamCollection = TeamService.GetAll();

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

        /// <summary>
        /// HR view request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Action result.</returns>
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
            request.TeamCollection = TeamService.GetAll();

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

        /// <summary>
        /// HR get leave request by date.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HRGetLeaveRequestByDate(RequestViewModel request)
        {
            IList<Leave> entityList = EmployeeService.GetLeaveRequestByDate(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get change shift request by date.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HRGetChangeShiftRequestByDate(RequestViewModel request)
        {
            IList<ChangingShift> entityList = EmployeeService.GetChangeShiftRequestByDate(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get late in / early out request by date.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HRGetLateInEarlyOutRequestByDate(RequestViewModel request)
        {
            IList<LateInEarlyOut> entityList = EmployeeService.GetLateInEarlyOutRequestByDate(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get check in/out request by date.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HRGetCheckInOutRequestByDate(RequestViewModel request)
        {
            IList<CheckInOut> entityList = EmployeeService.GetCheckInOutRequestByDate(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// Get all requests by date.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="role">The employee role.</param>
        public void GetAllRequestByDate(RequestViewModel request, short role)
        {
            IList<RequestEntity> entityList = EmployeeService.GetAllRequestByDate(
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

        /// <summary>
        /// HR get leave request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HRGetLeaveRequest(RequestViewModel request)
        {
            IList<Leave> entityList = EmployeeService.GetLeaveRequest(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get change shift request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HRGetChangeShiftRequest(RequestViewModel request)
        {
            IList<ChangingShift> entityList = EmployeeService.GetChangeShiftRequest(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get late in / early out request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HRGetLateInEarlyOutRequest(RequestViewModel request)
        {
            IList<LateInEarlyOut> entityList = EmployeeService.GetLateInEarlyOutRequest(
                   (short)RolesEnum.HR,
                   request.FromDate,
                   request.ToDate,
                   employeeId: request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                   groupId: request.TeamId,
                   status: request.StatusId != 0 ? new byte?(request.StatusId) : null,
                   logInId: null);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get check in/out request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HRGetCheckInOutRequest(RequestViewModel request)
        {
            IList<CheckInOut> entityList = EmployeeService.GetCheckInOutRequest(
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

        /// <summary>
        /// Get all request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="role">The employee role.</param>
        public void GetAllRequest(RequestViewModel request, short role)
        {
            IList<RequestEntity> entityList = EmployeeService.GetAllRequest(
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
