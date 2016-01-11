// --------------------------------------------------------------------------
// <copyright file="CheckInOutController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    [Authorize]
    public class CheckInOutController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IShiftService _shiftService;
        private readonly IRequestService _requestService;

        #region Ctor

        public CheckInOutController()
            : this(null, null, null)
        {
        }

        public CheckInOutController(
            IEmployeeService employeeService = null,
            IShiftService shiftService = null,
            IRequestService requestService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _shiftService = shiftService ?? new ShiftService();
            _requestService = requestService ?? new RequestService();
        }

        #endregion Ctor

        [HttpGet]
        public ActionResult EmployeeView()
        {
            return View();
        }

        [HttpPost]
        public JsonResult EmployeeView(int year)
        {
            ViewBag.Year = year;

            var criteria = new GetRequestCriteria()
            {
                Role = (short)Role.Member,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                EmployeeId = this.CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null,
                RequestType = Convert.ToString((int)RequestType.CheckIn) + "," + Convert.ToString((int)RequestType.CheckOut),
                IsFilterByEffectedDate = true
            };

            var data = _requestService.GetRequest(criteria);

            return Json(new { data = data }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Create(short? type = 1, DateTime? date = null)
        {
            var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = this.CurrentUser.Id,
                FromDate = Utilities.RequestAvailableFromDate,
                ToDate = Utilities.RequestAvailableToDate
            };

            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = this.CurrentUser.Id };
            var employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

            var viewModel = new CheckInOutRequestViewModel
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                ForDate = date.HasValue ? date.Value : DateTime.Today,
                Hour = DateTime.Now.Hour,
                Minute = DateTime.Now.Minute,
                CheckInOutType = type.HasValue ? type.Value : (short)RequestType.CheckIn,
                By = this.CurrentUser.Id,
                ShiftInfos = _shiftService.GetAll(new GetAllShiftCriteria()).ToList(),
                UnavailDates = _employeeService.GetDisableRequestDate(getUnavailableRequestDateCriteria).ToList()
            };
            var getShiftByDateCriteria = new GetShiftByDateCriteria()
            {
                EmployeeId = employee.Id,
                Date = DateTime.Now
            };

            var shift = _shiftService.GetByDate(getShiftByDateCriteria);
            if (shift != null)
            {
                viewModel.Hour = int.Parse(shift.BeginCheckInTime.Split(new[] { ':' })[0]);
                viewModel.Minute = int.Parse(shift.BeginCheckInTime.Split(new[] { ':' })[1]);
            }

            viewModel.Shift = (shift == null) ? string.Empty : shift.Name;

            int year = DateTime.Now.Year,
                month = DateTime.Now.Month;
            var criteria = new GetNumberRequestInMonthCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                RequestType = (int)RequestType.Leave
            };

            ViewBag.TotalRequest = _requestService.GetNumberRequestInMonth(criteria);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CheckInOutRequestViewModel model)
        {
            if (model.EmployeeId <= 0)
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidEmployeeID);
            }
            else
            {
                model.By = this.CurrentUser.Id;
                if (string.Compare(model.Employee, model.DisplayName, StringComparison.Ordinal) != 0)
                {
                    model.DisplayName = model.Employee;
                }

                model.ShiftInfos = _shiftService.GetAll(new GetAllShiftCriteria()).ToList();
                ViewBag.Requested = _requestService.CheckInOut(ModelState, model);
                ViewBag.Year = model.ForDate.Year;
            }

            return View(model);
        }

        public ActionResult Update(int id)
        {
            var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = this.CurrentUser.Id,
                FromDate = Utilities.RequestAvailableFromDate,
                ToDate = Utilities.RequestAvailableToDate
            };

            var getRequestByIdCriteria = new GetRequestByIdCriteria() { Id = id };
            var request = _requestService.GetRequestEntity(getRequestByIdCriteria);

            if (!this.Editable(request.Status.Id, request.Requester.Id))
            {
                return RedirectToAction("EmployeeView", "CheckInOut");
            }

            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = request.Requester.Id };
            var employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

            var getShiftByDateCriteria = new GetShiftByDateCriteria()
            {
                EmployeeId = employee.Id,
                Date = DateTime.Now
            };
            var shift = _shiftService.GetByDate(getShiftByDateCriteria);

            var model = new CheckInOutRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                ForDate = request.FromDate.Date,
                Hour = request.FromDate.Hour,
                Minute = request.FromDate.Minute,
                CheckInOutType = request.RequestType.Id,
                By = this.CurrentUser.Id,
                Shift = (shift == null) ? string.Empty : shift.Name,
                ShiftInfos = _shiftService.GetAll(new GetAllShiftCriteria()).ToList(),
                UnavailDates = _employeeService.GetDisableRequestDate(getUnavailableRequestDateCriteria).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(CheckInOutRequestViewModel model)
        {
            model.By = this.CurrentUser.Id;
            model.ShiftInfos = _shiftService.GetAll(new GetAllShiftCriteria()).ToList();
            ViewBag.Updated = _requestService.UpdateCheckInOut(ModelState, model);
            ViewBag.Year = model.ForDate.Year;

            return View(model);
        }

        public ActionResult Delete()
        {
            return View();
        }

        private bool Editable(byte status, int employeeId)
        {
            bool result = true;

            if (status != (byte)RequestStatuse.Pending || employeeId != this.CurrentUser.Id)
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
                EmployeeId = this.CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null,
                RequestType = Convert.ToString((int)RequestType.CheckIn) + "," + Convert.ToString((int)RequestType.CheckOut),
                IsFilterByEffectedDate = true
            };

            var entityList = _requestService.GetRequest(criteria);

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