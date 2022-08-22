namespace Fanex.NexHR.Controllers.Request
{
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels.CheckInOutRequest;

    public class CheckInOutController : BaseController
    {
        private readonly IRequestService requestService;
        private readonly IShiftService shiftService;
        private readonly ICommandService commandService;
        private readonly INexbotService nexbotService;
        private readonly IWebHostEnvironment environment;

        public CheckInOutController(ISessionService sessionService,
            IRequestService requestService,
            IShiftService shiftService,
            ICommandService commandService,
            INexbotService nexbotService,
            IWebHostEnvironment environment) : base(sessionService)
        {
            this.requestService = requestService;
            this.shiftService = shiftService;
            this.commandService = commandService;
            this.nexbotService = nexbotService;
            this.environment = environment;
        }

        public ActionResult EmployeeView()
        {
            return View();
        }

        public async Task<ActionResult> ListCheckInOutRequest(int year)
        {
            ViewBag.Year = year;

            var criteria = new GetRequestCriteria
            {
                Role = (short)Role.Member,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                EmployeeId = CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null,
                RequestType = Convert.ToString((int)RequestType.CheckIn) + "," +
                              Convert.ToString((int)RequestType.CheckOut),
                IsFilterByEffectedDate = true
            };

            IList<RequestEntity> checkInOutRequests =await requestService.GetRequest(criteria);

            return PartialView(checkInOutRequests);
        }

        public ActionResult Create(short? type = 1, DateTime? date = null, bool isCrossDate = false)
        {
            CheckInOutRequestViewModel viewModel = new CheckInOutRequestViewModel
            {
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                WorkDate = date ?? DateTime.Today,
                CheckInOutType = type ?? (short)RequestType.CheckIn,
                UnavailDates = requestService.GetDisableRequestDate(CurrentUser.Id, requestId: null),
                TotalRequest = requestService.GetNumberRequestInMonth(CurrentUser.Id, RequestType.CheckIn),
                IsCrossDate = isCrossDate
            };

            Shift shift = GetShiftByDate(date);
            if (shift != null)
            {
                viewModel.SetShiftIntoModel(shift);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CheckInOutRequestViewModel model)
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
                    CreatedDate = DateTime.Now,
                    CreatedBy = model.By,
                    FromDate = model.WorkDate
                };

                CommandResult result = commandService.ExecuteNonQuery(criteria);
                model.Id = criteria.Id;

                if (result.Code == 0 || result.Code == -1)
                {
                    ViewBag.Requested = true;
                    if (result.IsSuccessful)
                    {
                        model.IsEdit = false;
                        SendEmailAndSkype(model);
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

        private void SendEmailAndSkype(CheckInOutRequestViewModel model)
        {
            model.EmailTemplatePath = Path.Combine(environment.WebRootPath, Constants.EmailTemplatePathForCheckInOut);
            Task.Factory.StartNew(() =>
            {
                var leaders = requestService.GetLeaderForCheckInOut(model);
                if (leaders != null && leaders.Any())
                {
                    requestService.SendMailForCheckInOut(model, leaders);
                    nexbotService.SendMessageForCheckInOutRequest(model, leaders);
                }
            });
        }

        public async Task<ActionResult> Update(int id)
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
                UnavailDates = requestService.GetDisableRequestDate(CurrentUser.Id, requestId: null),
                IsCrossDate = request.IsCrossDate
            };

            Shift shift = GetShiftByDate(request.FromDate.Date);
            model.Shift = shift != null ? shift.DisplayName : string.Empty;

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(CheckInOutRequestViewModel model)
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
                    ModifiedDate = DateTime.Now,
                    FromShift = null,
                    ToDate = null,
                    ToShift = null,
                    LeaveTypeId = null,
                    TotalDays = null,
                    LateInMins = null,
                    EarlyOutMins = null,
                    IsCrossDate = model.IsCrossDate
                };

                CommandResult result = commandService.ExecuteNonQuery(criteria);

                if (result.Code == 0 || result.Code == -1)
                {
                    ViewBag.Updated = true;
                    if (result.Code == 0)
                    {
                        model.IsEdit = true;
                        SendEmailAndSkype(model);
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

        private Shift GetShiftByDate(DateTime? date = null)
        {
            var getShiftByDateCriteria = new GetShiftByDateCriteria
            {
                EmployeeId = CurrentUser.Id,
                Date = date ?? DateTime.Now
            };

            Shift shift = shiftService.GetByDate(getShiftByDateCriteria);
            return shift;
        }
    }
}