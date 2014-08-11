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
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Team;
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

            var criteria = new GetTeamByManagerIdCriteria()
            {
                ManagerId = this.CurrentUser.Id
            };
            request.TeamCollection = TeamService.GetTeamManagedByEmp(criteria);

            ViewBag.IsAllowChangeStatus = true;
            ViewBag.Status = request.StatusId;

            switch (request.RequestType)
            {
                case 1:
                    GetMemberLeaveRequest(request);
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
            var getLeaveRequestCriteria = new GetLeaveRequestCriteria
            {
                Role = (short)RolesEnum.AssistantLeader,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = this.CurrentUser.Id
            };

            IList<Leave> entityList = EmployeeService.GetLeaveRequest(getLeaveRequestCriteria);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// Get member late in / early out request.
        /// </summary>
        /// <param name="request">The request.</param>
        [NonAction]
        public void GetMemberLateInEarlyOut(RequestViewModel request)
        {
            var getLateInOutRequestCriteria = new GetLateInOutRequestCriteria
            {
                Role = (short)RolesEnum.AssistantLeader,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = this.CurrentUser.Id
            };

            IList<LateInEarlyOut> entityList = EmployeeService.GetLateInEarlyOutRequest(getLateInOutRequestCriteria);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// Get member check in/out request.
        /// </summary>
        /// <param name="request">The request.</param>
        [NonAction]
        public void GetMemberCheckInOut(RequestViewModel request)
        {
            var checkInOutRequestCriteria = new GetCheckInOutRequestCriteria
            {
                Role = (short)RolesEnum.AssistantLeader,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = this.CurrentUser.Id
            };

            IList<CheckInOut> entityList = EmployeeService.GetCheckInOutRequest(checkInOutRequestCriteria);

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
            var getLeaveRequestHistoryCriteria = new GetRequestHistoryCriteria
            {
                RequestId = requestId,
                RequestType = requestType
            };

            IList<Leave> entityList = EmployeeService.GetLeaveRequestHistory(getLeaveRequestHistoryCriteria);
            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get late in / early out request history..
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <param name="requestType">The request type.</param>
        public void HRGetLateInEarlyOutRequestHistory(long requestId, int requestType)
        {
            var getLateInOutRequestCriteria = new GetRequestHistoryCriteria
            {
                RequestId = requestId,
                RequestType = requestType
            };

            IList<LateInEarlyOut> entityList = EmployeeService.GetLateInEarlyOutRequestHistory(getLateInOutRequestCriteria);
            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get check in/out request history..
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <param name="requestType">The request type.</param>
        public void HRGetCheckInOutRequestHistory(long requestId, int requestType)
        {
            var getCheckInOutRequestCriteria = new GetRequestHistoryCriteria
            {
                RequestId = requestId,
                RequestType = requestType
            };

            IList<CheckInOut> entityList = EmployeeService.GetCheckInOutRequestHistory(getCheckInOutRequestCriteria);
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
            request.TeamCollection = TeamService.GetAll(new GetAllTeamCriteria());

            // request.StatusId = 0;
            ViewBag.Status = request.StatusId;

            switch (request.RequestType)
            {
                case 1:
                    HRGetLeaveRequestByDate(request);
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
            request.TeamCollection = TeamService.GetAll(new GetAllTeamCriteria());

            // request.StatusId = 0;
            ViewBag.Status = request.StatusId;

            switch (request.RequestType)
            {
                case 1:
                    HRGetLeaveRequest(request);
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
            request.TeamCollection = TeamService.GetAll(new GetAllTeamCriteria());

           // request.StatusId = 0;
            ViewBag.Status = request.StatusId;

            switch (request.RequestType)
            {
                case 1:
                    HRGetLeaveRequest(request);
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
            var leaveRequestByDateCriteria = new GetLeaveRequestByDateCriteria
            {
                Role = (short)RolesEnum.HR,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = null
            };

            IList<Leave> entityList = EmployeeService.GetLeaveRequestByDate(leaveRequestByDateCriteria);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get late in / early out request by date.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HRGetLateInEarlyOutRequestByDate(RequestViewModel request)
        {
            var getLateInOutRequestByDateCriteria = new GetLateInOutRequestByDateCriteria
            {
                Role = (short)RolesEnum.HR,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = null
            };

            IList<LateInEarlyOut> entityList = EmployeeService.GetLateInEarlyOutRequestByDate(getLateInOutRequestByDateCriteria);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get check in/out request by date.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HRGetCheckInOutRequestByDate(RequestViewModel request)
        {
            var getCheckInOutRequestByDateCriteria = new GetCheckInOutRequestByDateCriteria
            {
                Role = (short)RolesEnum.HR,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = null
            };

            IList<CheckInOut> entityList = EmployeeService.GetCheckInOutRequestByDate(getCheckInOutRequestByDateCriteria);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// Get all requests by date.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="role">The employee role.</param>
        public void GetAllRequestByDate(RequestViewModel request, short role)
        {
            var getAllRequestByDateCriteria = new GetAllRequestByDateCriteria
            {
                Role = role,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = role == (short)RolesEnum.AssistantLeader ? new int?(this.CurrentUser.Id) : null
            };
            IList<RequestEntity> entityList = EmployeeService.GetAllRequestByDate(getAllRequestByDateCriteria);

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
            var getLeaveRequestCriteria = new GetLeaveRequestCriteria
            {
                Role = (short)RolesEnum.HR,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = null
            };

            IList<Leave> entityList = EmployeeService.GetLeaveRequest(getLeaveRequestCriteria);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get late in / early out request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HRGetLateInEarlyOutRequest(RequestViewModel request)
        {
            var getLateInOutRequestCriteria = new GetLateInOutRequestCriteria
            {
                Role = (short)RolesEnum.HR,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = null
            };

            IList<LateInEarlyOut> entityList = EmployeeService.GetLateInEarlyOutRequest(getLateInOutRequestCriteria);

            ViewBag.EntityList = entityList;
        }

        /// <summary>
        /// HR get check in/out request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void HRGetCheckInOutRequest(RequestViewModel request)
        {
            var checkInOutRequestCriteria = new GetCheckInOutRequestCriteria
            {
                Role = (short)RolesEnum.HR,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = null
            };

            IList<CheckInOut> entityList = EmployeeService.GetCheckInOutRequest(checkInOutRequestCriteria);

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
            var getAllRequestCriteria = new GetAllRequestCriteria()
            {
                Role = role,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = role == (short)RolesEnum.AssistantLeader ? new int?(this.CurrentUser.Id) : null
            };

            IList<RequestEntity> entityList = EmployeeService.GetAllRequest(getAllRequestCriteria);

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
