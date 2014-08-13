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
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Check in/out controller.
    /// </summary>
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

        #endregion

        /// <summary>
        /// Create check in/out request.
        /// </summary>
        /// <param name="type">Check in or check out.</param>
        /// <param name="date">Request date.</param>
        /// <returns>Action result.</returns>
        public ActionResult Create(short? type = 1, DateTime? date = null)
        {
            var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = this.CurrentUser.Id,
                FromDate = Utilities.RequestAvailableFromDate,
                ToDate = Utilities.RequestAvailableToDate
            };

            ViewBag.UnavailDates = _employeeService.GetDisableRequestDate(getUnavailableRequestDateCriteria);
            ViewBag.Shifts = _shiftService.GetAll(new GetAllShiftCriteria());
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
                By = this.CurrentUser.Id
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
            int totalRequestInMonth = CountRequestInMonth();
            ViewBag.TotalRequest = totalRequestInMonth;

            return View(viewModel);
        }

        /// <summary>
        /// Create check in/out request.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Create(CheckInOutRequestViewModel model)
        {
            ViewBag.Shifts = _shiftService.GetAll(new GetAllShiftCriteria());

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

                ViewBag.Requested = _requestService.CheckInOut(ModelState, model);
                ViewBag.Year = model.ForDate.Year;
            }

            return View(model);
        }

        /// <summary>
        /// Update check in/out request.
        /// </summary>
        /// <param name="id">The check in/out request ID.</param>
        /// <returns>Action result.</returns>
        public ActionResult Update(int id)
        {
            var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = this.CurrentUser.Id,
                FromDate = Utilities.RequestAvailableFromDate,
                ToDate = Utilities.RequestAvailableToDate
            };

            var listUnavailDates = _employeeService.GetDisableRequestDate(getUnavailableRequestDateCriteria);
            ViewBag.UnavailDates = listUnavailDates;

            ViewBag.Shifts = _shiftService.GetAll(new GetAllShiftCriteria());
            var getWorkingTimeRequestCriteria = new GetWorkingTimeRequestCriteria() { Id = id };
            var request = _employeeService.GetWorkingTimeRequest(getWorkingTimeRequestCriteria);

            if (!this.Editable(request.Status, request.EmpId, request.CreatedBy))
            {
                return RedirectToAction("EmployeeView", "CheckInOut");
            }

            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = request.EmpId };
            var employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

            var getShiftByDateCriteria = new GetShiftByDateCriteria()
            {
                EmployeeId = employee.Id,
                Date = DateTime.Now
            };
            var shift = _shiftService.GetByDate(getShiftByDateCriteria);
            return View(new CheckInOutRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                ForDate = request.FromDate.Date,
                Hour = request.FromDate.Hour,
                Minute = request.FromDate.Minute,
                CheckInOutType = request.RequestType,
                By = this.CurrentUser.Id,
                Shift = (shift == null) ? string.Empty : shift.Name
            });
        }

        /// <summary>
        /// Update check in/out request.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Update(CheckInOutRequestViewModel model)
        {
            model.By = this.CurrentUser.Id;
            ViewBag.Shifts = _shiftService.GetAll(new GetAllShiftCriteria());
            ViewBag.Updated = _requestService.UpdateCheckInOut(ModelState, model);
            ViewBag.Year = model.ForDate.Year;
            return View(model);
        }

        /// <summary>
        /// Delete check in/out request.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Delete()
        {
            return View();
        }

        /// <summary>
        /// Employee view.
        /// </summary>
        /// <returns>Action result.</returns>
        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        /// <summary>
        /// Get check in/out request of a specific employee in a year.
        /// </summary>
        /// <param name="year">The input year.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;
            var checkInOutRequestCriteria = new GetCheckInOutRequestCriteria
            {
                Role = (short)Role.Member,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                EmployeeId = this.CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null
            };

            IList<CheckInOut> entityList = _employeeService.GetCheckInOutRequest(checkInOutRequestCriteria);

            return View(entityList);
        }

        /// <summary>
        /// Check if user can and has permission to edit a request.
        /// </summary>
        /// <param name="status">Status of request.</param>
        /// <param name="employeeId">Which employee request belong to.</param>
        /// <param name="creatorId">The person who creates this request.</param>
        /// <returns>Return true if current user can edit this request.</returns>
        private bool Editable(byte status, int employeeId, int creatorId)
        {
            if (status != (byte)RequestStatuse.Pending ||
                (employeeId != this.CurrentUser.Id && creatorId != this.CurrentUser.Id))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Count number of Check in/out request in current month.
        /// </summary>
        /// <returns>Number of request.</returns>
        private int CountRequestInMonth()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int numberOfRequest = 0;
            var checkInOutRequestCriteria = new GetCheckInOutRequestCriteria
            {
                Role = (short)Role.Member,
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                EmployeeId = this.CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null
            };

            IList<CheckInOut> entityList = _employeeService.GetCheckInOutRequest(checkInOutRequestCriteria);
            foreach (CheckInOut request in entityList)
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
