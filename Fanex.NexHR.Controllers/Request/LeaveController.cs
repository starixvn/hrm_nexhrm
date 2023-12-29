namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Criteria.Request;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Request;
    using Resources;
    using Services.Interfaces;
    using ViewModels;

    public class LeaveController : BaseController
    {
        private readonly ILeaveTypeService leaveTypeService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly INexbotService nexbotService;
        private readonly IWebHostEnvironment environment;

        private const int BusinessTripLeaveType = 9;
        private const int BusinessLeaveLeaveType = 32;

        public LeaveController(ISessionService sessionService,
            ILeaveTypeService leaveTypeService,
            IRequestService requestService,
            IBaseService baseService,
            INexbotService nexbotService,
            IWebHostEnvironment environment) : base(sessionService)
        {
            this.leaveTypeService = leaveTypeService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.nexbotService = nexbotService;
            this.environment = environment;
        }

        private const double day = 0.5;

        public IActionResult EmployeeView()
        {
            return View();
        }

        public async Task<IActionResult> ListLeaveRequest(int year)
        {
            var criteria = new GetRequestCriteria
            {
                Role = (short)Role.Member,
                FromDate = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(year, 12, 31, 0, 0, 0, TimeSpan.Zero).DateTime,
                EmployeeId = CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null,
                RequestType = Convert.ToString((int)RequestType.Leave),
                IsFilterByEffectedDate = true
            };

            IList<RequestEntity> leaveRequests = await requestService.GetRequest(criteria);

            return PartialView(leaveRequests);
        }

        public async Task<IActionResult> Create(DateTime? fromDate, DateTime? toDate)
        {
            var model = new LeaveRequestViewModel
            {
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                FromDate = fromDate ?? DateTimeOffset.Now.Date,
                ToDate = toDate ?? DateTimeOffset.Now.Date,
                TotalDays = 1,
                LeaveTypes = await leaveTypeService.GetAll(),
                UnavailDates = await requestService.GetDisableRequestDate(CurrentUser.Id, requestId: null),
                TotalRequest = await requestService.GetNumberRequestInMonth(CurrentUser.Id, RequestType.Leave)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LeaveRequestViewModel model)
        {
            var leaveRequestViewModel = model;
            if (leaveRequestViewModel == null)
            {
                return View((LeaveRequestViewModel)null);
            }

            leaveRequestViewModel = await PrepareModelCreate(leaveRequestViewModel);

            if (!ModelState.IsValid || leaveRequestViewModel.EmployeeId == 0)
            {
                return View(leaveRequestViewModel);
            }

            if (string.IsNullOrEmpty(leaveRequestViewModel.Remark) || string.IsNullOrWhiteSpace(leaveRequestViewModel.Remark))
            {
                ModelState.AddModelError(string.Empty, Messages.RemarkRequireError);
                return View(model);
            }

            var criteria = new InsertRequestCriteria
            {
                EmpId = leaveRequestViewModel.EmployeeId,
                RequestType = (short)RequestType.Leave,
                IsPlanned = leaveRequestViewModel.IsPlanned,
                Remark = leaveRequestViewModel.Remark,
                CreatedDate = DateTimeOffset.Now.DateTime,
                CreatedBy = CurrentUser.Id,
                FromDate = leaveRequestViewModel.FromDate,
                ToDate = leaveRequestViewModel.ToDate,
                LeaveTypeId = leaveRequestViewModel.LeaveTypeId,
                TotalDays = leaveRequestViewModel.TotalDays
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
            model.Id = criteria.Id;
            if (result.Code == 0 || result.Code == -1)
            {
                ViewBag.Requested = true;
                if (result.Code != 0)
                {
                    return View(model);
                }

                model.IsEdit = false;
                SendEmailAndSkype(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
            }

            return View(model);
        }

        private void SendEmailAndSkype(LeaveRequestViewModel model)
        {
            var templatePath = model.IsUnpaidLeave || model.IsAnnualLeaveOver5Day
                        ? Constants.EmailTemplatePathForUnpaidLeave
                        : Constants.EmailTemplatePathForLeave;

            model.EmailTemplatePath = Path.Combine(environment.ContentRootPath, templatePath);

            model.CurrentUserEmail = CurrentUser.Email;
            Task.Factory.StartNew(async () =>
            {
                await requestService.SendMailForLeave(model);
                await nexbotService.SendMessageForLeaveRequest(model);
            });
        }

        public async Task<IActionResult> Update(int id)
        {
            var criteria = new GetRequestByIdCriteria { Id = id };
            RequestEntity request = await requestService.GetRequestEntity(criteria);
            var fromHourSub = request.FromDate.Hour;
            var toHourSub = request.ToDate.Hour;
            var fromMinutesSub = request.FromDate.Minute;
            var toMinutesSub = request.ToDate.Minute;
            var isShowBoxTime = request.LeaveType.Id == BusinessTripLeaveType || request.LeaveType.Id == BusinessLeaveLeaveType;

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
                LeaveTypes = await leaveTypeService.GetAll(),
                UnavailDates = await requestService.GetDisableRequestDate(CurrentUser.Id, requestId: id),
                FromHour = isShowBoxTime && request.TotalDays == (decimal)0.5 ? fromHourSub : 0,
                ToHour = isShowBoxTime && request.TotalDays == (decimal)0.5 ? toHourSub : 0,
                FromMinute = isShowBoxTime && request.TotalDays == (decimal)0.5 ? fromMinutesSub : 0,
                ToMinute = isShowBoxTime && request.TotalDays == (decimal)0.5 ? toMinutesSub : 0,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(LeaveRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var viewModel = await PrepareModel(model);
                var criteria = new UpdateRequestCriteria
                {
                    Id = viewModel.Id,
                    IsPlanned = viewModel.IsPlanned,
                    Remark = viewModel.Remark,
                    ModifiedBy = viewModel.EmployeeId,
                    FromDate = viewModel.FromDate,
                    ModifiedDate = DateTimeOffset.Now.DateTime,
                    ToDate = viewModel.ToDate,
                    LeaveTypeId = viewModel.LeaveTypeId,
                    TotalDays = viewModel.TotalDays
                };

                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.Code == 0 || result.Code == -1)
                {
                    ViewBag.Updated = true;
                    if (result.Code == 0)
                    {
                        viewModel.IsEdit = true;
                        SendEmailAndSkype(viewModel);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
                return View(viewModel);
            }
            else
            {
                if (model != null)
                {
                    model.LeaveTypes = await leaveTypeService.GetAll();
                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAnnualLeaveBalanceInMonth(int empId, DateTime effectedDate, long? requestId)
        {
            var criteria = new GetLeaveBalanceInMonthCriteria
            {
                EmpId = empId,
                EffectedDate = effectedDate,
                RequestId = requestId
            };

            var leaveBalanceInMonth = await requestService.GetAnnualLeaveBalanceInMonth(criteria);

            return Json(leaveBalanceInMonth);
        }

        private async Task<LeaveRequestViewModel> PrepareModelCreate(LeaveRequestViewModel model)
        {
            var viewModel = await PrepareModel(model);
            viewModel.UnavailDates = await requestService.GetDisableRequestDate(CurrentUser.Id, requestId: null);
            ViewBag.Year = model.FromDate.Year;
            return viewModel;
        }

        private async Task<LeaveRequestViewModel> PrepareModel(LeaveRequestViewModel model)
        {
            model.LeaveTypes = await leaveTypeService.GetAll();
            var fromTimeLeaveBusiness = string.Concat(model.FromHour, ":", model.FromMinute);
            var toTimeLeaveBusiness = string.Concat(model.ToHour, ":", model.ToMinute);
            var isShowBoxTime = model.LeaveTypeId == BusinessTripLeaveType || model.LeaveTypeId == BusinessLeaveLeaveType;
            var fromDate = isShowBoxTime ? Utilities.AddTimeToDate(model.FromDate, fromTimeLeaveBusiness) : Utilities.AddTimeToDate(model.FromDate, model.FromTime);
            var toDate = isShowBoxTime ? Utilities.AddTimeToDate(model.ToDate, toTimeLeaveBusiness) : Utilities.AddTimeToDate(model.ToDate, model.ToTime);
            if (model.HalfDayOff)
            {
                model.TotalDays = (decimal)day;
                model.FromDate = fromDate;
                model.ToDate = toDate;
            }

            return model;
        }
    }
}