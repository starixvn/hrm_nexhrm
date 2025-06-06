namespace Fanex.NexHR.Controllers.Request
{
    using Core;
    using Core.Enums;
    using Criteria.Request;
    using Criteria.Team;
    using Fanex.NexHR.Controllers.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Request;
    using Resources;
    using Services;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels;
    using ViewModels.Common;

    public class RequestController : BaseController
    {
        private readonly IRequestService requestService;
        private readonly ITeamService teamService;
        private readonly ILeaveTypeService leaveTypeService;
        private readonly IBaseService baseService;

        public RequestController(ISessionService sessionService,
            IRequestService requestService,
            ITeamService teamService,
            ILeaveTypeService leaveTypeService,
            IBaseService baseService) : base(sessionService)
        {
            this.requestService = requestService;
            this.teamService = teamService;
            this.leaveTypeService = leaveTypeService;
            this.baseService = baseService;
        }

        private const int months = 2;
        private const int numberId = 2;

        [HrmAuthorize(UserRoles = "AssistantLeader,HR")]
        [HttpPost("request/process-request")]
        public async Task<IActionResult> ProcessRequest(string command, long id)
        {
            CommandResult result = await requestService.Process(command, id, CurrentUser.Id);
            return Json(result);
        }

        [HrmAuthorize(UserRoles = "AssistantLeader,HR")]
        [HttpPost]
        public async Task<IActionResult> ApproveMultiple(int[] requestIds)
        {
            var commandResult = new CommandResult();
            if (requestIds.Length > 0)
            {
                foreach (var requestId in requestIds)
                {
                    var result = await requestService.Process(Command.Approve.ToString(), requestId, CurrentUser.Id);
                    if (!result.IsSuccessful)
                    {
                        commandResult.Code = result.Code;
                        commandResult.Message = result.Message;
                    }
                }
            }

            return Json(commandResult);
        }

        [HrmAuthorize(UserRoles = "AssistantLeader,HR")]
        [HttpPost]
        public async Task<IActionResult> RejectMultiple(int[] requestIds)
        {
            var commandResult = new CommandResult();
            if (requestIds.Length > 0)
            {
                foreach (var requestId in requestIds)
                {
                    var result = await requestService.Process(Command.Reject.ToString(), requestId, CurrentUser.Id);
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

        [HrmAuthorize(UserRoles = "AssistantLeader")]
        public async Task<IActionResult> Member(RequestViewModel model)
        {
            if (model.TeamId == -1)
            {
                model = new RequestViewModel
                {
                    DateOption = 0,
                    ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
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
            model.Teams = await GetTeamByLeaders();
            model.LeaveTypes = await GetLeaveTypes();

            return View(model);
        }

        [HrmAuthorize(UserRoles = "AssistantLeader")]
        public async Task<IActionResult> GetAllRequestByAssistantLeader(RequestViewModel model)
        {
            model.Requests = await GetRequest(model, (short)Role.AssistantLeader);
            return PartialView("AllRequest", model);
        }

        public async Task<IActionResult> EmployeeView(RequestViewModel model)
        {
            var newModel = await GetRequestViewModel(model, RequestViewType.StaffView);

            return View("HRView", newModel);
        }

        public async Task<IActionResult> GetAllRequestByEmployee(RequestViewModel model)
        {
            if (model != null)
            {
                model.Requests = await GetRequest(model, (short)Role.HR);
            }

            return PartialView("AllRequest", model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> HRView(RequestViewModel model)
        {
            var newModel = await GetRequestViewModel(model, RequestViewType.HRView);

            return View(newModel);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> GetAllRequestByHR(RequestViewModel model)
        {
            if (model != null)
            {
                model.Requests = await GetRequest(model, (short)Role.HR);

                ViewBag.Status = model.StatusId;
                ViewBag.IsAllowChangeStatus = true;
                ViewBag.Leader = false;
                ViewBag.Options = model;
            }

            return PartialView("AllRequest", model);
        }

        public async Task<IActionResult> ViewHistory(long requestId, int requestType)
        {
            var criteria = new GetRequestHistoryCriteria
            {
                RequestId = requestId
            };

            IList<RequestEntity> requests = await requestService.GetRequestHistory(criteria);

            ViewBag.requestType = requestType;

            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWorkingTimeRequest(int id)
        {
            var criteria = new DeleteRequestCriteria
            {
                Id = id
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

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

        public async Task<JsonResult> GetUnavailDate(int employeeId)
        {
            IEnumerable<DateTime> dates = await requestService.GetDisableRequestDate(employeeId, requestId: null);
            return Json(dates.Select(t => t.ToString("dd MMM yyyy")));
        }

        public async Task<JsonResult> GetDateRange(int employeeId, DateTime fromDate, DateTime toDate, long? requestId)
        {
            var getDateRangeCriteria = new GetDateRangeEmployeeCalendarCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate,
                RequestId = requestId == 0 ? null : requestId
            };

            DateRange totalDays = await requestService.GetDateRange(getDateRangeCriteria);

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
                model.ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone();
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

        private async Task<RequestViewModel> GetRequestViewModel(RequestViewModel model, RequestViewType requestViewType)
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
                ? await GetTeamByLeaders() : await teamService.GetTeam();
            model.LeaveTypes = await GetLeaveTypes();

            return model;
        }

        private async Task<IList<RequestEntity>> GetRequest(RequestViewModel request, short role)
        {
            var criteria = new GetRequestCriteria
            {
                ActorId = CurrentUser.Id,
                Role = role,
                FromDate = request.FromDate.ToServerTimeZone(),
                ToDate = request.ToDate.ToServerTimeZone(),
                EmployeeId = request.EmployeeId != -1 ? new int?(request.EmployeeId) : null,
                GroupId = request.TeamId != 0 ? request.TeamId : null,
                LeaveTypeId = request.LeaveTypeId == 0 ? null : new int?(request.LeaveTypeId),
                Status = request.StatusId != 0 ? new byte?(request.StatusId) : null,
                ExcludeEmployeeId = role == (short)Role.AssistantLeader ? new int?(CurrentUser.Id) : null,
                RequestType = GetRequestTypeName(request.RequestType),
                IsFilterByEffectedDate = request.IsFilterByEffectedDate,
                PageIndex = request.CurrentPage - 1,
                ItemsPerPage = request.ItemsPerPage
            };

            IList<RequestEntity> entityList = await requestService.GetRequest(criteria);

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

        private async Task<IEnumerable<LeaveType>> GetLeaveTypes()
        {
            var leaveTypes = await leaveTypeService.GetAll();
            var result = leaveTypes.ToList();
            result.Insert(0, new LeaveType { Id = 0, Name = Labels.All });

            return result;
        }

        private async Task<IEnumerable<Team>> GetTeamByLeaders()
        {
            var criteria = new GetTeamByManagerIdCriteria
            {
                ManagerId = CurrentUser.Id
            };
            var teams = await teamService.GetTeamManagedByEmp(criteria);

            return teams;
        }
    }
}