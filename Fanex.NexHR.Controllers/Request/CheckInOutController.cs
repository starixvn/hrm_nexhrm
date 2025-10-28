namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Enums;
    using Criteria.Request;
    using Criteria.Shift;
    using Fanex.NexHR.Core;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Request;
    using Resources;
    using Services.Interfaces;
    using ViewModels.CheckInOutRequest;

    public class CheckInOutController : BaseController
    {
        private readonly IRequestService requestService;
        private readonly IShiftService shiftService;
        private readonly IBaseService baseService;

        public CheckInOutController(ISessionService sessionService,
            IRequestService requestService,
            IShiftService shiftService,
            IBaseService baseService) : base(sessionService)
        {
            this.requestService = requestService;
            this.shiftService = shiftService;
            this.baseService = baseService;
        }

        public IActionResult EmployeeView()
        {
            return View();
        }

        public async Task<IActionResult> ListCheckInOutRequest(int year)
        {
            ViewBag.Year = year;

            var criteria = new GetRequestCriteria
            {
                Role = (short)Role.Member,
                FromDate = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(year, 12, 31, 0, 0, 0, TimeSpan.Zero).DateTime,
                EmployeeId = CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null,
                RequestType = Convert.ToString((int)RequestType.CheckIn) + "," +
                              Convert.ToString((int)RequestType.CheckOut),
                IsFilterByEffectedDate = true
            };

            IList<RequestEntity> checkInOutRequests = await requestService.GetRequest(criteria);

            return PartialView(checkInOutRequests);
        }

        public async Task<IActionResult> Create(short? type = 1, DateTime? date = null, bool isCrossDate = false)
        {
            var viewModel = new CheckInOutRequestViewModel
            {
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                WorkDate = date ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Date,
                CheckInOutType = type ?? (short)RequestType.CheckIn,
                UnavailDates = await requestService.GetDisableRequestDate(CurrentUser.Id, requestId: null),
                TotalRequest = await requestService.GetNumberRequestInMonth(CurrentUser.Id, RequestType.CheckIn),
                IsCrossDate = isCrossDate
            };

            Shift shift = await GetShiftByDate(date);
            if (shift != null)
            {
                viewModel.SetShiftIntoModel(shift);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CheckInOutRequestViewModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return View(model);
            }

            model.By = CurrentUser.Id;
            model.WorkDate = model.WorkDate + new TimeSpan(model.Hour, model.Minute, 0);

            if (model.EmployeeId != 0)
            {
                var criteria = new InsertRequestCriteria
                {
                    EmpId = model.EmployeeId,
                    RequestType = model.CheckInOutType,
                    IsPlanned = false,
                    IsCrossDate = model.IsCrossDate,
                    CreatedDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
                    CreatedBy = model.By,
                    FromDate = model.WorkDate
                };

                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
                model.Id = criteria.Id;

                if (result.Code == 0 || result.Code == -1)
                {
                    ViewBag.Requested = true;
                    if (result.IsSuccessful)
                    {
                        model.IsEdit = false;
                        requestService.SendEmailAndChatBotForCheckInOut(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Labels.InvalidEmployee);
            }

            ViewBag.Year = model.WorkDate.Year;

            return View(model);
        }

        public async Task<IActionResult> Update(int id)
        {
            var criteria = new GetRequestByIdCriteria { Id = id };
            RequestEntity request = await requestService.GetRequestEntity(criteria);

            if (!Editable(request.Status.Id, request.Requester.Id))
            {
                return RedirectToAction("EmployeeView", "CheckInOut");
            }

            var model = new CheckInOutRequestViewModel
            {
                Id = request.Id,
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                WorkDate = request.FromDate.Date,
                Hour = request.FromDate.Hour,
                Minute = request.FromDate.Minute,
                CheckInOutType = request.RequestType.Id,
                UnavailDates = await requestService.GetDisableRequestDate(CurrentUser.Id, requestId: null),
                IsCrossDate = request.IsCrossDate
            };

            Shift shift = await GetShiftByDate(request.FromDate.Date);
            model.Shift = shift != null ? shift.DisplayName : string.Empty;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CheckInOutRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                model.By = CurrentUser.Id;
                model.WorkDate = model.WorkDate + new TimeSpan(model.Hour, model.Minute, 0);
                var criteria = new UpdateRequestCriteria
                {
                    Id = model.Id,
                    IsPlanned = false,
                    Remark = model.Remark,
                    ModifiedBy = model.EmployeeId,
                    FromDate = model.WorkDate,
                    ModifiedDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
                    FromShift = null,
                    ToDate = null,
                    ToShift = null,
                    LeaveTypeId = null,
                    TotalDays = null,
                    LateInMins = null,
                    EarlyOutMins = null,
                    IsCrossDate = model.IsCrossDate
                };

                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

                if (result.Code == 0 || result.Code == -1)
                {
                    ViewBag.Updated = true;
                    if (result.Code == 0)
                    {
                        model.IsEdit = true;
                        requestService.SendEmailAndChatBotForCheckInOut(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }

                ViewBag.Year = model.WorkDate.Year;
            }

            return View(model);
        }

        private bool Editable(byte status, int employeeId)
        {
            bool result = !(status != (byte)RequestStatuse.Pending
                            || employeeId != CurrentUser.Id);

            return result;
        }

        private async Task<Shift> GetShiftByDate(DateTime? date = null)
        {
            var getShiftByDateCriteria = new GetShiftByDateCriteria
            {
                EmployeeId = CurrentUser.Id,
                Date = date ?? DateTimeOffset.Now.DateTime.ToClientTimeZone()
            };

            Shift shift = await shiftService.GetByDate(getShiftByDateCriteria);
            return shift;
        }
    }
}