namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.CheckInOutRequest;

    public class CheckInOutController : BaseController
    {
        private readonly IRequestService requestService;
        private readonly IShiftService shiftService;
        private readonly ICommandService commandService;

        public CheckInOutController()
            : this(null, null, null)
        {
        }

        public CheckInOutController(
            IRequestService requestService,
            IShiftService shiftService,
            ICommandService commandService)
        {
            this.requestService = requestService ?? new RequestService();
            this.shiftService = shiftService ?? new ShiftService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult EmployeeView()
        {
            return View();
        }

        public ActionResult ListCheckInOutRequest(int year)
        {
            ViewBag.Year = year;

            var criteria = new GetRequestCriteria()
            {
                Role = (short)Role.Member,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                EmployeeId = CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null,
                RequestType = Convert.ToString((int)RequestType.CheckIn) + "," + Convert.ToString((int)RequestType.CheckOut),
                IsFilterByEffectedDate = true
            };

            IList<RequestEntity> checkInOutRequests = requestService.GetRequest(criteria);

            return PartialView(checkInOutRequests);
        }

        public ActionResult Create(short? type = 1, DateTime? date = null)
        {
            CheckInOutRequestViewModel viewModel = new CheckInOutRequestViewModel() 
            { 
                Employee = CurrentUser.DisplayName,
                EmployeeId = CurrentUser.Id,
                ForDate = date.HasValue ? date.Value : DateTime.Today,
                CheckInOutType = type.HasValue ? type.Value : (short)RequestType.CheckIn,
                UnavailDates = requestService.GetDisableRequestDate(CurrentUser.Id),
                TotalRequest = requestService.GetNumberRequestInMonth(CurrentUser.Id, RequestType.CheckIn)
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
            if (model != null && ModelState.IsValid)
            {
                model.By = CurrentUser.Id;
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);

                var criteria = new InsertRequestCriteria()
                {
                    EmpId = model.EmployeeId,
                    RequestType = model.CheckInOutType,
                    IsPlanned = false,
                    Remark = null,
                    CreatedDate = DateTime.Now,
                    CreatedBy = model.By,
                    FromDate = model.ForDate,
                    FromShift = null,
                    ToDate = null,
                    ToShift = null,
                    LeaveTypeId = null,
                    TotalDays = null,
                    LateInMins = null,
                    EarlyOutMins = null
                };

                CommandResult result = commandService.Insert(criteria, hasGetOutputParams: true);
                model.Id = criteria.Id;

                if (result.IsSuccessful)
                {
                    ViewBag.Requested = true;

                    model.IsEdit = false;
                    model.GetEmailTemplatePath();
                    var taskSendMail = new Task(() => requestService.SendMailForCheckInOut(model));
                    taskSendMail.Start();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }

                ViewBag.Year = model.ForDate.Year;
            }

            return View(model);
        }

        public ActionResult Update(int id)
        {
            var criteria = new GetRequestByIdCriteria() { Id = id };
            RequestEntity request = requestService.GetRequestEntity(criteria);

            if (!this.Editable(request.Status.Id, request.Requester.Id))
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
                UnavailDates = requestService.GetDisableRequestDate(CurrentUser.Id)
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
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);
                var criteria = new UpdateRequestCriteria()
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

                CommandResult result = commandService.Insert(criteria, hasGetOutputParams: true);

                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;

                    model.IsEdit = true;
                    model.GetEmailTemplatePath();
                    var taskSendMail = new Task(() => requestService.SendMailForCheckInOut(model));
                    taskSendMail.Start();
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
            bool result = true;

            if (status != (byte)RequestStatuse.Pending || employeeId != CurrentUser.Id)
            {
                result = false;
            }

            return result;
        }

        private Shift GetShiftByDate(DateTime? date = null)
        {
            var getShiftByDateCriteria = new GetShiftByDateCriteria()
            {
                EmployeeId = CurrentUser.Id,
                Date = date.HasValue ? date.Value : DateTime.Now
            };

            Shift shift = shiftService.GetByDate(getShiftByDateCriteria);
            return shift;
        }
    }
}