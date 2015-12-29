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
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// Request controller.
    /// </summary>
    [Authorize]
    public class RequestController : BaseController
    {
        #region private services
        private readonly IRequestService _requestService;
        private readonly ITeamService _teamService;
        private readonly ILeaveTypeService _leaveTypeService;
        #endregion

        #region Ctor
        public RequestController()
            : this(null, null, null)
        {
        }

        public RequestController(
            IRequestService requestService = null,
            ITeamService teamService = null,
            ILeaveTypeService leaveTypeService = null)
        {
            _requestService = requestService ?? new RequestService();
            _teamService = teamService ?? new TeamService();
            _leaveTypeService = leaveTypeService ?? new LeaveTypeService();
        }
        #endregion

        public PartialViewResult CriteriaPanel(RequestViewModel request)
        {
            return PartialView(request);
        }

        /// <summary>
        /// View member request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "AssistantLeader")]
        public ActionResult Member(RequestViewModel request)
        {
            if (request.TeamId == -1)
            {
                request = new RequestViewModel
                {
                    DateOption = 0,
                    ToDate = DateTime.Now,
                    FromDate = request.ToDate.AddMonths(-2),
                    RequestType = 5,
                    TeamId = null,
                    IsFilterByEffectedDate = false
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
            request.TeamCollection = _teamService.GetTeamManagedByEmp(criteria);
            GetRequest(request, (short)Role.AssistantLeader);

            request.FilteringLeaveTypes = _leaveTypeService.GetAll().ToList();
            request.FilteringLeaveTypes.Insert(0, new LeaveType() { Id = 0, Name = Labels.All });

            ViewBag.IsAllowChangeStatus = true;
            ViewBag.Status = request.StatusId;
            ViewBag.Leader = true;
            ViewBag.Options = request;

            return View(request);
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
                    TeamId = null,
                    IsFilterByEffectedDate = true
                };
            }

            // Get request model from Request monitoring view.
            if (request.TeamId == -2)
            {
                request.StatusId = 0;
                request.TeamId = null;
            }

            request.TeamCollection = _teamService.GetAll(new GetAllTeamCriteria());
            GetRequest(request, (short)Role.HR);

            ViewBag.Status = request.StatusId;
            ViewBag.IsAllowChangeStatus = false;
            ViewBag.Leader = false;
            ViewBag.Options = request;
            ViewBag.LeaveTypes = _leaveTypeService.GetAll();

            return View("HRView", request);
        }

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
                    IsFilterByEffectedDate = false
                };
            }

            // Get request model from Request monitoring view.
            if (request.TeamId == -2)
            {
                request.StatusId = 0;
                request.TeamId = null;
            }

            request.TeamCollection = _teamService.GetAll(new GetAllTeamCriteria());
            GetRequest(request, (short)Role.HR);

            request.FilteringLeaveTypes = _leaveTypeService.GetAll().ToList();
            request.FilteringLeaveTypes.Insert(0, new LeaveType() { Id = 0, Name = Labels.All });

            ViewBag.Status = request.StatusId;
            ViewBag.IsAllowChangeStatus = true;
            ViewBag.Leader = false;
            ViewBag.Options = request;

            return View(request);
        }

        public ActionResult ViewHistory(long requestId, int requestType)
        {
            var criteria = new GetRequestHistoryCriteria
            {
                RequestId = requestId
            };

            IList<RequestEntity> requests = _requestService.GetRequestHistory(criteria);
            ViewBag.EntityList = requests;
            ViewBag.requestType = requestType;
            return View();
        }

        [HttpPost]
        public bool DeleteWorkingTimeRequest(int id)
        {
            var criteria = new DeleteRequestCriteria() 
            { 
                Id = id 
            };
            bool deleted = _requestService.DeleteRequest(criteria);
            return deleted;
        }

        private void GetRequest(RequestViewModel request, short role)
        {
            var criteria = new GetRequestCriteria()
            {
                Role = role,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                LeaveTypeId = request.LeaveTypeId == 0 ? null : new int?(request.LeaveTypeId),
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = role == (short)Role.AssistantLeader ? new int?(this.CurrentUser.Id) : null,
                RequestType = request.RequestType == 1
                    ? Convert.ToString((int)RequestType.Leave)
                    : (request.RequestType == 4 ? (Convert.ToString((int)RequestType.CheckIn) + "," + Convert.ToString((int)RequestType.CheckOut)) : string.Empty),
                IsFilterByEffectedDate = request.IsFilterByEffectedDate
            };

            IList<RequestEntity> entityList = _requestService.GetRequest(criteria);

            ViewBag.EntityList = entityList;
        }
    }
}
