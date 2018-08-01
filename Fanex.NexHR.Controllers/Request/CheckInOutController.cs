namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.Enums;
    using Criteria.Request;
    using Criteria.Shift;
    using Models;
    using Models.Request;
    using Resources;
    using Services;
    using Services.Interfaces;
    using ViewModels.CheckInOutRequest;

    public class CheckInOutController : BaseController
    {
        private readonly IRequestService _requestService;
        private readonly IShiftService _shiftService;
        private readonly ICommandService _commandService;
        private readonly INexbotService _nexbotService;

        public CheckInOutController()
            : this(null, null, null, null)
        {
        }

        public CheckInOutController(
            IRequestService requestService,
            IShiftService shiftService,
            ICommandService commandService,
            INexbotService nexbotService)
        {
            _requestService = requestService ?? new RequestService();
            _shiftService = shiftService ?? new ShiftService();
            _commandService = commandService ?? new CommandService();
            _nexbotService = nexbotService ?? new NexbotService();
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult EmployeeView()
        {
            return View();
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult ListCheckInOutRequest(int year)
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

            IList<RequestEntity> checkInOutRequests = _requestService.GetRequest(criteria);

            return PartialView(checkInOutRequests);
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult Create(short? type = 1, DateTime? date = null)
        {
            CheckInOutRequestViewModel viewModel = new CheckInOutRequestViewModel
            {
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                ForDate = date ?? DateTime.Today,
                CheckInOutType = type ?? (short)RequestType.CheckIn,
                UnavailDates = _requestService.GetDisableRequestDate(CurrentUser.Id),
                TotalRequest = _requestService.GetNumberRequestInMonth(CurrentUser.Id, RequestType.CheckIn)
            };

            Shift shift = GetShiftByDate(date);
            if (shift != null)
            {
                viewModel.SetShiftIntoModel(shift);
            }

            return View(viewModel);
        }

        [Authorize(Roles = "SpecialUser")]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(CheckInOutRequestViewModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return View(model);
            }

            model.By = CurrentUser.Id;
            model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);

            if (model.EmployeeId != 0)
            {
                var criteria = new InsertRequestCriteria
                {
                    EmpId = model.EmployeeId,
                    RequestType = model.CheckInOutType,
                    IsPlanned = false,
                    CreatedDate = DateTime.Now,
                    CreatedBy = model.By,
                    FromDate = model.ForDate
                };

                CommandResult result = _commandService.Insert(criteria, hasGetOutputParams: true);
                model.Id = criteria.Id;

                if (result.Code == 0 || result.Code == -1)
                {
                    ViewBag.Requested = true;
                    if (result.Code == 0)
                    {
                        model.IsEdit = false;
                        model.GetTemplatePath();
                        Task.Factory.StartNew(() =>
                        {
                            _requestService.SendMailForCheckInOut(model);
                            _nexbotService.SendMessageForCheckInOutRequest(model);
                        });
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

            ViewBag.Year = model.ForDate.Year;

            return View(model);
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult Update(int id)
        {
            var criteria = new GetRequestByIdCriteria { Id = id };
            RequestEntity request = _requestService.GetRequestEntity(criteria);

            if (!Editable(request.Status.Id, request.Requester.Id))
            {
                return RedirectToAction("EmployeeView", "CheckInOut");
            }

            var model = new CheckInOutRequestViewModel
            {
                Id = request.Id,
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                ForDate = request.FromDate.Date,
                Hour = request.FromDate.Hour,
                Minute = request.FromDate.Minute,
                CheckInOutType = request.RequestType.Id,
                UnavailDates = _requestService.GetDisableRequestDate(CurrentUser.Id)
            };

            Shift shift = GetShiftByDate(request.FromDate.Date);
            model.Shift = shift != null ? shift.DisplayName : string.Empty;

            return View(model);
        }

        [Authorize(Roles = "SpecialUser")]
        [HttpPost]
        public ActionResult Update(CheckInOutRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                model.By = CurrentUser.Id;
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);
                var criteria = new UpdateRequestCriteria
                {
                    Id = model.Id,
                    IsPlanned = false,
                    Remark = model.Remark,
                    ModifiedBy = model.EmployeeId,
                    FromDate = model.ForDate,
                    ModifiedDate = DateTime.Now,
                    FromShift = null,
                    ToDate = null,
                    ToShift = null,
                    LeaveTypeId = null,
                    TotalDays = null,
                    LateInMins = null,
                    EarlyOutMins = null
                };

                CommandResult result = _commandService.Insert(criteria, hasGetOutputParams: true);

                if (result.Code == 0 || result.Code == -1)
                {
                    ViewBag.Updated = true;
                    if (result.Code == 0)
                    {
                        model.IsEdit = true;
                        model.GetTemplatePath();
                        Task.Factory.StartNew(() =>
                        {
                            _requestService.SendMailForCheckInOut(model);
                            _nexbotService.SendMessageForCheckInOutRequest(model);
                        });
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }

                ViewBag.Year = model.ForDate.Year;
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

            Shift shift = _shiftService.GetByDate(getShiftByDateCriteria);
            return shift;
        }
    }
}