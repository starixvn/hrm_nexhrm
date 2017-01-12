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

    public class RequestController : BaseController
    {
        private readonly IRequestService requestService;
        private readonly ITeamService teamService;
        private readonly ILeaveTypeService leaveTypeService;
        private readonly ICommandService commandService;

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
            this.requestService = requestService ?? new RequestService();
            this.teamService = teamService ?? new TeamService();
            this.leaveTypeService = leaveTypeService ?? new LeaveTypeService();
            this.commandService = commandService ?? new CommandService();
        }

        [Authorize(Roles = "AssistantLeader,HR,SpecialUser")]
        [HttpPost]
        public JsonResult ProcessRequest(string command, long id)
        {
            CommandResult result = requestService.Process(command, id, CurrentUser.Id);
            return Json(result);
        }

        public PartialViewResult CriteriaPanel(RequestViewModel request)
        {
            return PartialView(request);
        }

        [Authorize(Roles = "AssistantLeader,SpecialUser")]
        public ActionResult Member(RequestViewModel request)
        {
            if (request != null)
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
                request.Teams = teamService.GetTeamManagedByEmp(criteria);

                request.LeaveTypes = leaveTypeService.GetAll();
                request.LeaveTypes.Insert(0, new LeaveType() { Id = 0, Name = Labels.All });

                ViewBag.ActionName = "GetAllRequestByAssistantLeader";
            }

            return View(request);
        }

        [ValidateInput(false)]
        [Authorize(Roles = "AssistantLeader,SpecialUser")]
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
            if (request != null)
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

                request.Teams = teamService.GetTeam();

                request.LeaveTypes = leaveTypeService.GetAll();
                request.LeaveTypes.Insert(0, new LeaveType() { Id = 0, Name = Labels.All });

                ViewBag.ActionName = "GetAllRequestByEmployee";
            }

            return View("HRView", request);
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
            if (request != null)
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

                request.Teams = teamService.GetTeam();

                request.LeaveTypes = leaveTypeService.GetAll();
                request.LeaveTypes.Insert(0, new LeaveType() { Id = 0, Name = Labels.All });

                ViewBag.ActionName = "GetAllRequestByHR";
            }

            return View(request);
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

        [Authorize(Roles = "SpecialUser")]
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
            var criteria = new DeleteRequestCriteria()
            {
                Id = id
            };
            CommandResult result = commandService.Delete(criteria);

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
            IList<DateTime> dates = requestService.GetDisableRequestDate(employeeId);
            return Json(dates.Select(t => t.ToString("dd MMM yyyy")), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var getDateRangeCriteria = new GetDateRangeEmployeeCalendarCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            DateRange totalDays = requestService.GetDateRange(getDateRangeCriteria);

            return Json(totalDays.TotalDay, JsonRequestBehavior.AllowGet);
        }

        private IList<RequestEntity> GetRequest(RequestViewModel request, short role)
        {
            var criteria = new GetRequestCriteria()
            {
                ActorId = CurrentUser.Id,
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

            IList<RequestEntity> entityList = requestService.GetRequest(criteria);

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