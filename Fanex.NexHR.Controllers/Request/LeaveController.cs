namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Criteria.Request;
    using Models;
    using Models.Request;
    using Services;
    using Services.Interfaces;
    using ViewModels;
    using Resources;
    using System.Globalization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Hosting;
    using System.IO;

    public class LeaveController : BaseController
    {
        private readonly ILeaveTypeService leaveTypeService;
        private readonly IRequestService requestService;
        private readonly ICommandService commandService;
        private readonly INexbotService nexbotService;
        private readonly IWebHostEnvironment environment;

        private const int BusinessTripLeaveType = 9;

        public LeaveController(ISessionService sessionService,
            ILeaveTypeService leaveTypeService,
            IRequestService requestService,
            ICommandService commandService,
            INexbotService nexbotService,
            IWebHostEnvironment environment) : base(sessionService)
        {
            this.leaveTypeService = leaveTypeService;
            this.requestService = requestService;
            this.commandService = commandService;
            this.nexbotService = nexbotService;
            this.environment = environment;
        }
        private const double day = 0.5;

        public ActionResult EmployeeView()
        {
            return View();
        }

        public async Task<ActionResult> ListLeaveRequest(int year)
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

            IList<RequestEntity> leaveRequests = await requestService.GetRequest(criteria);

            return PartialView(leaveRequests);
        }

        public ActionResult Create(DateTime? fromDate, DateTime? toDate)
        {
            var model = new LeaveRequestViewModel
            {
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                FromDate = fromDate ?? DateTime.Now.Date,
                ToDate = toDate ?? DateTime.Now.Date,
                TotalDays = 1,
                LeaveTypes = leaveTypeService.GetAll(),
                UnavailDates = requestService.GetDisableRequestDate(CurrentUser.Id, requestId: null),
                TotalRequest = requestService.GetNumberRequestInMonth(CurrentUser.Id, RequestType.Leave)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(LeaveRequestViewModel model)
        {
            var leaveRequestViewModel = model;
            if (leaveRequestViewModel == null)
            {
                return View((LeaveRequestViewModel)null);
            }

            leaveRequestViewModel = PrepareModelCreate(leaveRequestViewModel);

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
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUser.Id,
                FromDate = leaveRequestViewModel.FromDate,
                ToDate = leaveRequestViewModel.ToDate,
                LeaveTypeId = leaveRequestViewModel.LeaveTypeId,
                TotalDays = leaveRequestViewModel.TotalDays
            };

            CommandResult result = commandService.ExecuteNonQuery(criteria);
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

            model.EmailTemplatePath = Path.Combine(environment.WebRootPath, templatePath);

            model.CurrentUserEmail = CurrentUser.Email;
            Task.Factory.StartNew(() =>
            {
                requestService.SendMailForLeave(model);
                nexbotService.SendMessageForLeaveRequest(model);
            });
        }

        public async Task<ActionResult> Update(int id)
        {
            var criteria = new GetRequestByIdCriteria { Id = id };
            RequestEntity request = await requestService.GetRequestEntity(criteria);
            var fromHourSub = request.FromDate.Hour;
            var toHourSub = request.ToDate.Hour;
            var fromMinutesSub = request.FromDate.Minute;
            var toMinutesSub = request.ToDate.Minute;

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
                LeaveTypes = leaveTypeService.GetAll(),
                UnavailDates = requestService.GetDisableRequestDate(CurrentUser.Id, requestId: id),
                FromHour = request.LeaveType.Id == BusinessTripLeaveType && request.TotalDays == (decimal)0.5 ? fromHourSub : 0,
                ToHour = request.LeaveType.Id == BusinessTripLeaveType && request.TotalDays == (decimal)0.5 ? toHourSub : 0,
                FromMinute = request.LeaveType.Id == BusinessTripLeaveType && request.TotalDays == (decimal)0.5 ? fromMinutesSub : 0,
                ToMinute = request.LeaveType.Id == BusinessTripLeaveType && request.TotalDays == (decimal)0.5 ? toMinutesSub : 0,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(LeaveRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var viewModel = PrepareModel(model);
                var criteria = new UpdateRequestCriteria
                {
                    Id = viewModel.Id,
                    IsPlanned = viewModel.IsPlanned,
                    Remark = viewModel.Remark,
                    ModifiedBy = viewModel.EmployeeId,
                    FromDate = viewModel.FromDate,
                    ModifiedDate = DateTime.Now,
                    ToDate = viewModel.ToDate,
                    LeaveTypeId = viewModel.LeaveTypeId,
                    TotalDays = viewModel.TotalDays
                };

                CommandResult result = commandService.ExecuteNonQuery(criteria);
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
                    model.LeaveTypes = leaveTypeService.GetAll();
                }
                return View(model);
            }

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

            var leaveBalanceInMonth = requestService.GetAnnualLeaveBalanceInMonth(criteria);

            return Json(leaveBalanceInMonth);
        }

        private LeaveRequestViewModel PrepareModelCreate(LeaveRequestViewModel model)
        {
            var viewModel = PrepareModel(model);
            viewModel.UnavailDates = requestService.GetDisableRequestDate(CurrentUser.Id, requestId: null);
            ViewBag.Year = model.FromDate.Year;
            return viewModel;
        }

        private LeaveRequestViewModel PrepareModel(LeaveRequestViewModel model)
        {
            model.LeaveTypes = leaveTypeService.GetAll();
            var fromTimeLeaveBusiness = string.Concat(model.FromHour, ":", model.FromMinute);
            var toTimeLeaveBusiness = string.Concat(model.ToHour, ":", model.ToMinute);
            var fromDate = model.LeaveTypeId == BusinessTripLeaveType ? Utilities.AddTimeToDate(model.FromDate, fromTimeLeaveBusiness) : Utilities.AddTimeToDate(model.FromDate, model.FromTime);
            var toDate = model.LeaveTypeId == BusinessTripLeaveType ? Utilities.AddTimeToDate(model.ToDate, toTimeLeaveBusiness) : Utilities.AddTimeToDate(model.ToDate, model.ToTime);
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