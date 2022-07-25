namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Enums;
    using Criteria.Request;
    using Criteria.Team;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Request;
    using Resources;
    using Services;
    using Services.Interfaces;
    using ViewModels;
    using ViewModels.Common;

    public class RequestController : BaseController
    {
        private readonly IRequestService requestService;
        private readonly ITeamService teamService;
        private readonly ILeaveTypeService leaveTypeService;
        private readonly ICommandService commandService;

        public RequestController(ISessionService sessionService,
            IRequestService requestService,
            ITeamService teamService,
            ILeaveTypeService leaveTypeService,
            ICommandService commandService) : base(sessionService)
        {
            this.requestService = requestService;
            this.teamService = teamService;
            this.leaveTypeService = leaveTypeService;
            this.commandService = commandService;
        }
        private const int months = 2;
        private const int numberId = 2;

        [Authorize(Roles = "AssistantLeader,HR")]
        [HttpPost("request/process-request")]
        public JsonResult ProcessRequest(string command, long id)
        {
            CommandResult result = requestService.Process(command, id, CurrentUser.Id);
            return Json(result);
        }

        [Authorize(Roles = "AssistantLeader,HR")]
        [HttpPost]
        public JsonResult ApproveMultiple(int[] requestIds)
        {
            var commandResult = new CommandResult();
            if (requestIds.Length > 0)
            {
                foreach(var requestId in requestIds)
                {
                    var result = requestService.Process(Command.Approve.ToString(), requestId, CurrentUser.Id);
                    if (!result.IsSuccessful)
                    {
                        commandResult.Code = result.Code;
                        commandResult.Message = result.Message;
                    }
                }    
            }

            return Json(commandResult);
        }

        [Authorize(Roles = "AssistantLeader,HR")]
        [HttpPost]
        public JsonResult RejectMultiple(int[] requestIds)
        {
            var commandResult = new CommandResult();
            if (requestIds.Length > 0)
            {
                foreach (var requestId in requestIds)
                {
                    var result = requestService.Process(Command.Reject.ToString(), requestId, CurrentUser.Id);
                    if (!result.IsSuccessful)
                    {
                        commandResult.Code = result.Code;
                        commandResult.Message = result.Message;
                    }
                }
            }

            return Json(commandResult);
        }

        public PartialViewResult CriteriaPanel(RequestViewModel model)
        {
            return PartialView(model);
        }

        [Authorize(Roles = "AssistantLeader")]
        public ActionResult Member(RequestViewModel model)
        {
            if (model.TeamId == -1)
            {
                model = new RequestViewModel
                {
                    DateOption = 0,
                    ToDate = DateTime.Now,
                    FromDate = model.ToDate.AddMonths(-months),
                    RequestType = (int)RequestTypeFilterForRequest.All,
                    TeamId = null,
                    IsFilterByEffectedDate = false,
                    RequestViewType = RequestViewType.LeaderView
                };
            }

            if (model.TeamId == -numberId)
            {
                model.TeamId = null;
            }

            model.CurrentPage = Constants.CurrentPage;
            model.ItemsPerPage = Constants.ItemsPerPage;
            model.Teams = GetTeamByLeaders();
            model.LeaveTypes = GetLeaveTypes();

            return View(model);
        }

        [Authorize(Roles = "AssistantLeader")]
        public ActionResult GetAllRequestByAssistantLeader(RequestViewModel model)
        {
            model.Requests = GetRequest(model, (short)Role.AssistantLeader);
            return PartialView("AllRequest", model);
        }

        public ActionResult EmployeeView(RequestViewModel model)
        {
            var newModel = GetRequestViewModel(model, RequestViewType.StaffView);

            return View("HRView", newModel);
        }

        public ActionResult GetAllRequestByEmployee(RequestViewModel model)
        {
            if (model != null)
            {
                model.Requests = GetRequest(model, (short)Role.HR);
            }

            return PartialView("AllRequest", model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult HRView(RequestViewModel model)
        {
            var newModel = GetRequestViewModel(model, RequestViewType.HRView);

            return View(newModel);
        }

        [Authorize(Roles = "HR")]
        public ActionResult GetAllRequestByHR(RequestViewModel model)
        {
            if (model != null)
            {
                model.Requests = GetRequest(model, (short)Role.HR);

                ViewBag.Status = model.StatusId;
                ViewBag.IsAllowChangeStatus = true;
                ViewBag.Leader = false;
                ViewBag.Options = model;
            }

            return PartialView("AllRequest", model);
        }

        public ActionResult ViewHistory(long requestId, int requestType)
        {
            var criteria = new GetRequestHistoryCriteria
            {
                RequestId = requestId
            };

            IList<RequestEntity> requests = requestService.GetRequestHistory(criteria);

            ViewBag.requestType = requestType;

            return View(requests);
        }

        [HttpPost]
        public ActionResult DeleteWorkingTimeRequest(int id)
        {
            var criteria = new DeleteRequestCriteria
            {
                Id = id
            };
            CommandResult result = commandService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [HttpPost]
        public string GetRequestDetail(RequestEntity request, short requestType, string fromShiftCode, string toShiftCode)
        {
            if (request != null)
            {
                request.RequestType = new RequestTypeModel { Id = requestType };
                request.FromShift = new Shift { Name = fromShiftCode };
                request.ToShift = new Shift { Name = toShiftCode };
            }

            string requestDetail = RequestService.ConvertToRequestMessage(request);
            return requestDetail;
        }

        public JsonResult GetUnavailDate(int employeeId)
        {
            IList<DateTime> dates = requestService.GetDisableRequestDate(employeeId, requestId: null);
            return Json(dates.Select(t => t.ToString("dd MMM yyyy")));
        }

        public JsonResult GetDateRange(int employeeId, DateTime fromDate, DateTime toDate, long? requestId)
        {
            var getDateRangeCriteria = new GetDateRangeEmployeeCalendarCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate,
                RequestId = requestId == 0 ? null : requestId
            };

            DateRange totalDays = requestService.GetDateRange(getDateRangeCriteria);

            return Json(totalDays.TotalDay);
        }

        private RequestViewModel GetDefaultRequestViewModel(RequestViewType requestViewType)
        {
            var model = new RequestViewModel
            {
                StatusId = 0,
                RequestType = (int)RequestTypeFilterForRequest.All,
                TeamId = null,
                RequestViewType = requestViewType
            };

            if (requestViewType == RequestViewType.LeaderView)
            {
                model.DateOption = 0;
                model.ToDate = DateTime.Now;
                model.FromDate = model.ToDate.AddMonths(-months);
                model.IsFilterByEffectedDate = false;
            }
            else if (requestViewType == RequestViewType.HRView)
            {
                model.IsFilterByEffectedDate = false;
            }
            else
            {
                model.IsFilterByEffectedDate = true;
            }

            return model;
        }

        private RequestViewModel GetRequestViewModel(RequestViewModel model, RequestViewType requestViewType)
        {
            if (model.TeamId == -1)
            {
                model = GetDefaultRequestViewModel(requestViewType);
            }

            if (model.TeamId == -numberId)
            {
                model.TeamId = null;
            }

            model.CurrentPage = Constants.CurrentPage;
            model.ItemsPerPage = Constants.ItemsPerPage;
            model.Teams = requestViewType == RequestViewType.LeaderView
                ? GetTeamByLeaders() : teamService.GetTeam();
            model.LeaveTypes = GetLeaveTypes();

            return model;
        }

        private IList<RequestEntity> GetRequest(RequestViewModel request, short role)
        {
            var criteria = new GetRequestCriteria
            {
                ActorId = CurrentUser.Id,
                Role = role,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId,
                LeaveTypeId = request.LeaveTypeId == 0 ? null : new int?(request.LeaveTypeId),
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = role == (short)Role.AssistantLeader ? new int?(CurrentUser.Id) : null,
                RequestType = GetRequestTypeName(request.RequestType),
                IsFilterByEffectedDate = request.IsFilterByEffectedDate,
                PageIndex = request.CurrentPage - 1,
                ItemsPerPage = request.ItemsPerPage
            };

            IList<RequestEntity> entityList = requestService.GetRequest(criteria);

            if (criteria.TotalItem != null)
            {
                request.TotalItem = (int)criteria.TotalItem;
                var pagerOption = new PagerOptionModel
                {
                    CurrentPage = request.CurrentPage,
                    ItemsPerPage = request.ItemsPerPage,
                    TotalItem = (int)criteria.TotalItem
                };

                ViewBag.PagerOption = pagerOption;
            }

            return entityList;
        }

        private static string GetRequestTypeName(int requestType)
        {
            if (requestType == (int)RequestTypeFilterForRequest.Leave)
            {
                return Convert.ToString((int)RequestType.Leave);
            }

            return requestType == (int)RequestTypeFilterForRequest.CheckInOut 
                ? (Convert.ToString((int)RequestType.CheckIn) + "," + Convert.ToString((int)RequestType.CheckOut)) 
                : string.Empty;
        }

        private IList<LeaveType> GetLeaveTypes()
        {
            var leaveTypes = leaveTypeService.GetAll();
            leaveTypes.Insert(0, new LeaveType { Id = 0, Name = Labels.All });

            return leaveTypes;
        }

        private IList<Team> GetTeamByLeaders()
        {
            var criteria = new GetTeamByManagerIdCriteria
            {
                ManagerId = CurrentUser.Id
            };
            var teams = teamService.GetTeamManagedByEmp(criteria);

            return teams;
        }
    }
}