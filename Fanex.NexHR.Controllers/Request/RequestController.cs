namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core;
    using Core.Enums;
    using Criteria.Request;
    using Criteria.Team;
    using Models;
    using Models.Request;
    using Resources;
    using Services;
    using Services.Interfaces;
    using ViewModels;
    using ViewModels.Common;

    public class RequestController : BaseController
    {
        private readonly IRequestService _requestService;
        private readonly ITeamService _teamService;
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly ICommandService _commandService;

        public RequestController()
            : this(null, null, null, null)
        {
        }

        public RequestController(
            IRequestService requestService,
            ITeamService teamService,
            ILeaveTypeService leaveTypeService,
            ICommandService commandService)
        {
            _requestService = requestService ?? new RequestService();
            _teamService = teamService ?? new TeamService();
            _leaveTypeService = leaveTypeService ?? new LeaveTypeService();
            _commandService = commandService ?? new CommandService();
        }
        private const int months = 2;
        private const int numberType = 5;
        private const int numberId = 2;

        [Authorize(Roles = "AssistantLeader,HR")]
        [HttpPost]
        public JsonResult ProcessRequest(string command, long id)
        {
            CommandResult result = _requestService.Process(command, id, CurrentUser.Id);
            return Json(result);
        }

        public PartialViewResult CriteriaPanel(RequestViewModel request)
        {
            return PartialView(request);
        }

        [Authorize(Roles = "AssistantLeader")]
        public ActionResult Member(RequestViewModel request)
        {
            RequestViewModel modifiedRequest = null;
            if (request != null)
            {
                modifiedRequest = request;

                if (request.TeamId == -1)
                {
                    modifiedRequest = new RequestViewModel
                    {
                        DateOption = 0,
                        ToDate = DateTime.Now,
                        FromDate = request.ToDate.AddMonths(-months),
                        RequestType = numberType,
                        TeamId = null,
                        IsFilterByEffectedDate = false
                    };
                }

                if (request.TeamId == -numberId)
                {
                    modifiedRequest.TeamId = null;
                }

                modifiedRequest.CurrentPage = Constants.CurrentPage;
                modifiedRequest.ItemsPerPage = Constants.ItemsPerPage;

                var criteria = new GetTeamByManagerIdCriteria
                {
                    ManagerId = CurrentUser.Id
                };
                modifiedRequest.Teams = _teamService.GetTeamManagedByEmp(criteria);

                modifiedRequest.LeaveTypes = _leaveTypeService.GetAll();
                modifiedRequest.LeaveTypes.Insert(0, new LeaveType { Id = 0, Name = Labels.All });

                ViewBag.ActionName = "GetAllRequestByAssistantLeader";
            }

            return View(modifiedRequest);
        }

        [ValidateInput(false)]
        [Authorize(Roles = "AssistantLeader")]
        public ActionResult GetAllRequestByAssistantLeader(RequestViewModel request)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            if (request != null)
            {
                entityList = GetRequest(request, (short)Role.AssistantLeader);

                ViewBag.Status = request.StatusId;
                ViewBag.IsAllowChangeStatus = true;
                ViewBag.Leader = true;
                ViewBag.Options = request;
            }

            return PartialView("AllRequest", entityList);
        }

        public ActionResult EmployeeView(RequestViewModel request)
        {
            var modifiedRequest = request;
            if (modifiedRequest != null)
            {
                // Default view all request type
                if (modifiedRequest.TeamId == -1)
                {
                    modifiedRequest = new RequestViewModel
                    {
                        StatusId = 0,
                        RequestType = numberType,
                        TeamId = null,
                        IsFilterByEffectedDate = true
                    };
                }

                // Get request model from Request monitoring view.
                if (modifiedRequest.TeamId == -numberId)
                {
                    modifiedRequest.StatusId = 0;
                    modifiedRequest.TeamId = null;
                }

                modifiedRequest.CurrentPage = Constants.CurrentPage;
                modifiedRequest.ItemsPerPage = Constants.ItemsPerPage;

                modifiedRequest.Teams = _teamService.GetTeam();

                modifiedRequest.LeaveTypes = _leaveTypeService.GetAll();
                modifiedRequest.LeaveTypes.Insert(0, new LeaveType { Id = 0, Name = Labels.All });

                ViewBag.ActionName = "GetAllRequestByEmployee";
            }

            return View("HRView", modifiedRequest);
        }

        [ValidateInput(false)]
        public ActionResult GetAllRequestByEmployee(RequestViewModel request)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            if (request != null)
            {
                entityList = GetRequest(request, (short)Role.HR);

                ViewBag.Status = request.StatusId;
                ViewBag.IsAllowChangeStatus = false;
                ViewBag.Leader = false;
                ViewBag.Options = request;
            }

            return PartialView("AllRequest", entityList);
        }

        [Authorize(Roles = "HR")]
        public ActionResult HRView(RequestViewModel request)
        {
            var modifiedRequest = request;
            if (modifiedRequest != null)
            {
                // Default view all request type
                if (modifiedRequest.TeamId == -1)
                {
                    modifiedRequest = new RequestViewModel
                    {
                        StatusId = 0,
                        RequestType = numberType,
                        TeamId = null,
                        IsFilterByEffectedDate = false
                    };
                }

                // Get request model from Request monitoring view.
                if (modifiedRequest.TeamId == -numberId)
                {
                    modifiedRequest.StatusId = 0;
                    modifiedRequest.TeamId = null;
                }

                modifiedRequest.CurrentPage = Constants.CurrentPage;
                modifiedRequest.ItemsPerPage = Constants.ItemsPerPage;

                modifiedRequest.Teams = _teamService.GetTeam();

                modifiedRequest.LeaveTypes = _leaveTypeService.GetAll();
                modifiedRequest.LeaveTypes.Insert(0, new LeaveType { Id = 0, Name = Labels.All });

                ViewBag.ActionName = "GetAllRequestByHR";
            }

            return View(modifiedRequest);
        }

        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        public ActionResult GetAllRequestByHR(RequestViewModel request)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            if (request != null)
            {
                entityList = GetRequest(request, (short)Role.HR);

                ViewBag.Status = request.StatusId;
                ViewBag.IsAllowChangeStatus = true;
                ViewBag.Leader = false;
                ViewBag.Options = request;
            }

            return PartialView("AllRequest", entityList);
        }

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
        public ActionResult DeleteWorkingTimeRequest(int id)
        {
            var criteria = new DeleteRequestCriteria
            {
                Id = id
            };
            CommandResult result = _commandService.Delete(criteria);

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
            IList<DateTime> dates = _requestService.GetDisableRequestDate(employeeId, requestId: null);
            return Json(dates.Select(t => t.ToString("dd MMM yyyy")), JsonRequestBehavior.AllowGet);
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

            DateRange totalDays = _requestService.GetDateRange(getDateRangeCriteria);

            return Json(totalDays.TotalDay, JsonRequestBehavior.AllowGet);
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

            IList<RequestEntity> entityList = _requestService.GetRequest(criteria);

            if (criteria.TotalItem != null)
            {
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
    }
}