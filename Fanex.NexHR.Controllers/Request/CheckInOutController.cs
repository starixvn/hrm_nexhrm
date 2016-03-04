namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
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
            CheckInOutRequestViewModel viewModel = new CheckInOutRequestViewModel();
            var getShiftByDateCriteria = new GetShiftByDateCriteria()
            {
                EmployeeId = CurrentUser.Id,
                Date = date.HasValue ? date.Value : DateTime.Now
            };

            Shift shift = shiftService.GetByDate(getShiftByDateCriteria);
            if (shift != null)
            {
                var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
                {
                    EmployeeId = CurrentUser.Id,
                    FromDate = Utilities.RequestAvailableFromDate,
                    ToDate = Utilities.RequestAvailableToDate
                };

                viewModel.Employee = CurrentUser.DisplayName;
                viewModel.EmployeeId = CurrentUser.Id;
                viewModel.ForDate = date.HasValue ? date.Value : DateTime.Today;
                viewModel.Hour = type.HasValue && type.Value == 2
                    ? int.Parse(shift.BeginCheckOutTime.Split(new[] { ':' })[0])
                    : int.Parse(shift.BeginCheckInTime.Split(new[] { ':' })[0]);
                viewModel.Minute = type.HasValue && type.Value == 2
                        ? int.Parse(shift.BeginCheckOutTime.Split(new[] { ':' })[1])
                        : int.Parse(shift.BeginCheckInTime.Split(new[] { ':' })[1]);
                viewModel.Shift = shift.DisplayName;
                viewModel.InTime = shift.BeginCheckInTime;
                viewModel.OutTime = shift.BeginCheckOutTime;
                viewModel.CheckInOutType = type.HasValue ? type.Value : (short)RequestType.CheckIn;
                viewModel.UnavailDates = requestService.GetDisableRequestDate(getUnavailableRequestDateCriteria);

                int year = DateTime.Now.Year,
                    month = DateTime.Now.Month;
                var criteria = new GetNumberRequestInMonthCriteria()
                {
                    EmpId = CurrentUser.Id,
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    RequestType = (int)RequestType.CheckIn
                };
                viewModel.TotalRequest = requestService.GetNumberRequestInMonth(criteria);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CheckInOutRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                if (model.EmployeeId <= 0)
                {
                    ModelState.AddModelError(string.Empty, Messages.InvalidEmployeeID);
                }
                else
                {
                    model.By = CurrentUser.Id;

                    ViewBag.Requested = requestService.CreateCheckInOut(ModelState, model);
                    ViewBag.Year = model.ForDate.Year;
                }
            }

            return View(model);
        }

        public ActionResult Update(int id)
        {
            var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = CurrentUser.Id,
                FromDate = Utilities.RequestAvailableFromDate,
                ToDate = Utilities.RequestAvailableToDate
            };

            var getRequestByIdCriteria = new GetRequestByIdCriteria() { Id = id };
            var request = requestService.GetRequestEntity(getRequestByIdCriteria);

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
                UnavailDates = requestService.GetDisableRequestDate(getUnavailableRequestDateCriteria)
            };

            var getShiftByDateCriteria = new GetShiftByDateCriteria()
            {
                EmployeeId = model.EmployeeId,
                Date = request.FromDate.Date
            };

            Shift shift = shiftService.GetByDate(getShiftByDateCriteria);
            if (shift != null)
            {
                model.Shift = shift.DisplayName;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(CheckInOutRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                model.By = CurrentUser.Id;
                ViewBag.Updated = requestService.UpdateCheckInOut(ModelState, model);
                ViewBag.Year = model.ForDate.Year;
            }

            return View(model);
        }

        public ActionResult Delete()
        {
            return View();
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

        private int CountRequestInMonth()
        {
            int year = DateTime.Now.Year,
                month = DateTime.Now.Month,
                numberOfRequest = 0;

            var criteria = new GetRequestCriteria()
            {
                Role = (short)Role.Member,
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                EmployeeId = CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null,
                RequestType = Convert.ToString((int)RequestType.CheckIn) + "," + Convert.ToString((int)RequestType.CheckOut),
                IsFilterByEffectedDate = true
            };

            var entityList = requestService.GetRequest(criteria);

            foreach (RequestEntity request in entityList)
            {
                if (request.Status.Id != (byte)RequestStatuse.Reject)
                {
                    numberOfRequest++;
                }
            }

            return numberOfRequest;
        }
    }
}