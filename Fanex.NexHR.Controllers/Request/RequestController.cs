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
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Common;

    [Authorize]
    public class RequestController : BaseController
    {
        #region private services

        private readonly IRequestService _requestService;
        private readonly ITeamService _teamService;
        private readonly ILeaveTypeService _leaveTypeService;

        #endregion private services

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

        #endregion Ctor

        public PartialViewResult CriteriaPanel(RequestViewModel request)
        {
            return PartialView(request);
        }

        #region Get Request By Assistant Leader

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

            request.CurrentPage = Constants.CurrentPage;
            request.ItemsPerPage = Constants.ItemsPerPage;

            var criteria = new GetTeamByManagerIdCriteria()
            {
                ManagerId = this.CurrentUser.Id
            };
            request.Teams = _teamService.GetTeamManagedByEmp(criteria);

            request.LeaveTypes = _leaveTypeService.GetAll().ToList();
            request.LeaveTypes.Insert(0, new LeaveType() { Id = 0, Name = Labels.All });

            ViewBag.ActionName = "GetAllRequestByAssistantLeader";

            return View(request);
        }

        [Authorize(Roles = "AssistantLeader")]
        public ActionResult GetAllRequestByAssistantLeader(RequestViewModel request)
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

            IList<RequestEntity> entityList = GetRequest(request, (short)Role.AssistantLeader);

            ViewBag.Status = request.StatusId;
            ViewBag.IsAllowChangeStatus = true;
            ViewBag.Leader = true;
            ViewBag.Options = request;

            return PartialView("AllRequest", entityList);
        }

        #endregion Get Request By Assistant Leader

        #region Get Request By Employee

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

            request.CurrentPage = Constants.CurrentPage;
            request.ItemsPerPage = Constants.ItemsPerPage;

            request.Teams = _teamService.GetAll();

            request.LeaveTypes = _leaveTypeService.GetAll().ToList();
            request.LeaveTypes.Insert(0, new LeaveType() { Id = 0, Name = Labels.All });

            ViewBag.ActionName = "GetAllRequestByEmployee";

            return View("HRView", request);
        }

        public ActionResult GetAllRequestByEmployee(RequestViewModel request)
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

            IList<RequestEntity> entityList = GetRequest(request, (short)Role.HR);

            ViewBag.Status = request.StatusId;
            ViewBag.IsAllowChangeStatus = false;
            ViewBag.Leader = false;
            ViewBag.Options = request;

            return PartialView("AllRequest", entityList);
        }

        #endregion Get Request By Employee

        #region Get Request By HR

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

            request.CurrentPage = Constants.CurrentPage;
            request.ItemsPerPage = Constants.ItemsPerPage;

            request.Teams = _teamService.GetAll();

            request.LeaveTypes = _leaveTypeService.GetAll().ToList();
            request.LeaveTypes.Insert(0, new LeaveType() { Id = 0, Name = Labels.All });

            ViewBag.ActionName = "GetAllRequestByHR";

            return View(request);
        }

        [Authorize(Roles = "HR")]
        public ActionResult GetAllRequestByHR(RequestViewModel request)
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

            IList<RequestEntity> entityList = GetRequest(request, (short)Role.HR);

            ViewBag.Status = request.StatusId;
            ViewBag.IsAllowChangeStatus = true;
            ViewBag.Leader = false;
            ViewBag.Options = request;

            return PartialView("AllRequest", entityList);
        }

        #endregion Get Request By HR

        public ActionResult ViewHistory(long requestId, int requestType)
        {
            var criteria = new GetRequestHistoryCriteria
            {
                RequestId = requestId
            };

            IList<RequestEntity> requests = _requestService.GetRequestHistory(criteria);

            ViewBag.requestType = requestType;

            return View(requests);
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

        private IList<RequestEntity> GetRequest(RequestViewModel request, short role)
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
                IsFilterByEffectedDate = request.IsFilterByEffectedDate,
                PageIndex = request.CurrentPage - 1,
                ItemsPerPage = request.ItemsPerPage
            };

            IList<RequestEntity> entityList = _requestService.GetRequest(criteria);

            var pagerOption = new PagerOptionModel()
            {
                CurrentPage = request.CurrentPage,
                ItemsPerPage = request.ItemsPerPage,
                TotalItem = (int)criteria.TotalItem
            };

            ViewBag.PagerOption = pagerOption;

            return entityList;
        }
    }
}