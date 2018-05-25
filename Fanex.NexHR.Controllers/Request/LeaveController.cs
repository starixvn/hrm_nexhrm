namespace Fanex.NexHR.Controllers.Request
{
    using Core;
    using Core.Enums;
    using Criteria.Request;
    using Models;
    using Models.Request;
    using Services;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels;

    public class LeaveController : BaseController
    {
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly IRequestService _requestService;
        private readonly ICommandService _commandService;

        public LeaveController()
            : this(null, null, null)
        {
        }

        public LeaveController(
            ILeaveTypeService leaveTypeService,
            IRequestService requestService,
            ICommandService commandService)
        {
            _leaveTypeService = leaveTypeService ?? new LeaveTypeService();
            _requestService = requestService ?? new RequestService();
            _commandService = commandService ?? new CommandService();
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult EmployeeView()
        {
            return View();
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult ListLeaveRequest(int year)
        {
            var criteria = new GetRequestCriteria
            {
                Role = (short)Role.Member,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                EmployeeId = CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null,
                RequestType = Convert.ToString((int)RequestType.Leave),
                IsFilterByEffectedDate = true
            };

            IList<RequestEntity> leaveRequests = _requestService.GetRequest(criteria);

            return PartialView(leaveRequests);
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult Create()
        {
            var model = new LeaveRequestViewModel
            {
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1,
                LeaveTypes = _leaveTypeService.GetAll(),
                UnavailDates = _requestService.GetDisableRequestDate(CurrentUser.Id),
                TotalRequest = _requestService.GetNumberRequestInMonth(CurrentUser.Id, RequestType.Leave)
            };

            return View(model);
        }

        [Authorize(Roles = "SpecialUser")]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(LeaveRequestViewModel model)
        {
            var leaveRequestViewModel = model;
            if (leaveRequestViewModel == null)
            {
                return View((LeaveRequestViewModel)null);
            }

            leaveRequestViewModel = PrepareModel(leaveRequestViewModel);

            if (!ModelState.IsValid || leaveRequestViewModel.EmployeeId == 0)
            {
                return View(leaveRequestViewModel);
            }

            var criteria = new InsertRequestCriteria
            {
                EmpId = leaveRequestViewModel.EmployeeId,
                RequestType = (short)RequestType.Leave,
                IsPlanned = leaveRequestViewModel.IsPlanned,
                Remark = leaveRequestViewModel.Remark,
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUser.Id,
                FromDate = leaveRequestViewModel.FromDate,
                ToDate = leaveRequestViewModel.ToDate,
                LeaveTypeId = leaveRequestViewModel.LeaveTypeId,
                TotalDays = leaveRequestViewModel.TotalDays
            };

            CommandResult result = _commandService.Insert(criteria, hasGetOutputParams: true);
            model.Id = criteria.Id;
            if (result.Code == 0 || result.Code == -1)
            {
                ViewBag.Requested = true;
                if (result.Code != 0)
                {
                    return View(model);
                }

                model.IsEdit = false;
                model.GetTemplatePath();
                Task.Factory.StartNew(() =>
                {
                    _requestService.SendMailForLeave(model);
                });
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
            }

            return View(model);
        }

        private LeaveRequestViewModel PrepareModel(LeaveRequestViewModel model)
        {
            model.LeaveTypes = _leaveTypeService.GetAll();
            model.UnavailDates = _requestService.GetDisableRequestDate(CurrentUser.Id);
            ViewBag.Year = model.FromDate.Year;
            if (model.HalfDayOff)
            {
                model.TotalDays = (decimal)0.5;
                model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
            }

            return model;
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult Update(int id)
        {
            var criteria = new GetRequestByIdCriteria { Id = id };
            RequestEntity request = _requestService.GetRequestEntity(criteria);
            var model = new LeaveRequestViewModel
            {
                Id = request.Id,
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                FromDate = request.FromDate.Date,
                FromTime = Utilities.GetTime(request.FromDate),
                ToDate = request.ToDate,
                ToTime = Utilities.GetTime(request.ToDate),
                TotalDays = request.TotalDays,
                LeaveTypeId = request.LeaveType.Id,
                IsPlanned = request.IsPlanned,
                Remark = request.Remark,
                HalfDayOff = request.TotalDays == (decimal)0.5,
                LeaveTypes = _leaveTypeService.GetAll(),
                UnavailDates = _requestService.GetDisableRequestDate(CurrentUser.Id)
            };

            return View(model);
        }

        [Authorize(Roles = "SpecialUser")]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Update(LeaveRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                model.LeaveTypes = _leaveTypeService.GetAll();
                if (model.HalfDayOff)
                {
                    model.TotalDays = (decimal)0.5;
                    model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                    model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
                }

                var criteria = new UpdateRequestCriteria
                {
                    Id = model.Id,
                    IsPlanned = model.IsPlanned,
                    Remark = model.Remark,
                    ModifiedBy = model.EmployeeId,
                    FromDate = model.FromDate,
                    ModifiedDate = DateTime.Now,
                    ToDate = model.ToDate,
                    LeaveTypeId = model.LeaveTypeId,
                    TotalDays = model.TotalDays
                };

                CommandResult result = _commandService.Update(criteria);
                if (result.Code == 0 || result.Code == -1)
                {
                    ViewBag.Updated = true;
                    if (result.Code == 0)
                    {
                        model.IsEdit = true;
                        model.GetTemplatePath();
                        Task.Factory.StartNew(() =>
                        {
                            _requestService.SendMailForLeave(model);
                        });
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult GetAnnualLeaveBalanceInMonth(int empId, DateTime effectedDate, long? requestId)
        {
            var criteria = new GetLeaveBalanceInMonthCriteria
            {
                EmpId = empId,
                EffectedDate = effectedDate,
                RequestId = requestId
            };

            var leaveBalanceInMonth = _requestService.GetAnnualLeaveBalanceInMonth(criteria);

            return Json(leaveBalanceInMonth);
        }
    }
}