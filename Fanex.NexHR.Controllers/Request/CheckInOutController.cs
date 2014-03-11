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
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Check in/out controller.
    /// </summary>
    [Authorize]
    public class CheckInOutController : RequestController
    {
        #region Ctor

        public CheckInOutController()
            : base(null, null, null, null)
        {
        }

        #endregion Ctor

        /// <summary>
        /// Create check in/out request.
        /// </summary>
        /// <returns>Action result.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult Create()
        {
            ViewBag.UnavailDates = EmployeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.Shifts = ShiftService.GetAll(new GetAllShiftCriteria());
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = this.CurrentUser.Id };
            var employee = EmployeeService.Get(getEmployeeCriteria);
            var viewModel = new CheckInOutRequestViewModel
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                ForDate = DateTime.Today,
                Hour = DateTime.Now.Hour,
                Minute = DateTime.Now.Minute,
                Type = (short)RequestTypes.CheckIn,
                By = this.CurrentUser.Id
            };
            var getShiftByDateCriteria = new GetShiftByDateCriteria()
            {
                EmployeeId = employee.Id,
                Date = DateTime.Now
            };
            var shift = ShiftService.GetByDate(getShiftByDateCriteria);
            if (shift != null)
            {
                viewModel.Hour = int.Parse(shift.BeginCheckInTime.Split(new[] { ':' })[0]);
                viewModel.Minute = int.Parse(shift.BeginCheckInTime.Split(new[] { ':' })[1]);
            }

            viewModel.Shift = (shift == null) ? string.Empty : shift.Name;
            //// Count number of Check in/out request in current month.
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int numberOfRequest = 0;
            var checkInOutRequestCriteria = new GetCheckInOutRequestCriteria
            {
                Role = (short)RolesEnum.Member,
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                EmployeeId = this.CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null
            };

            IList<CheckInOut> entityList = EmployeeService.GetCheckInOutRequest(checkInOutRequestCriteria);
            foreach (CheckInOut request in entityList)
            {
                if (request.Status.Id != (byte)RequestStatuses.Reject)
                {
                    numberOfRequest++;
                }
            }

            ViewBag.TotalRequest = numberOfRequest;

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
            ViewBag.Shifts = ShiftService.GetAll(new GetAllShiftCriteria());

            if (model.EmployeeId <= 0)
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidEmployeeID);
            }
            else
            {
                model.By = this.CurrentUser.Id;
                if (string.Compare(model.Employee, model.DisplayName, true) != 0)
                {
                    model.DisplayName = model.Employee;
                }

                ViewBag.Requested = RequestService.CheckInOut(ModelState, model);
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
            var listUnavailDates = EmployeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;

            ViewBag.Shifts = ShiftService.GetAll(new GetAllShiftCriteria());
            var getWorkingTimeRequestCriteria = new GetWorkingTimeRequestCriteria() { Id = id };
            var request = EmployeeService.GetWorkingTimeRequest(getWorkingTimeRequestCriteria);

            if (!this.Editable(request.Status, request.EmpId, request.CreatedBy))
            {
                return RedirectToAction("EmployeeView", "CheckInOut");
            }

            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = request.EmpId };
            var employee = EmployeeService.Get(getEmployeeCriteria);

            var getShiftByDateCriteria = new GetShiftByDateCriteria()
            {
                EmployeeId = employee.Id,
                Date = DateTime.Now
            };
            var shift = ShiftService.GetByDate(getShiftByDateCriteria);
            return View(new CheckInOutRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                ForDate = request.FromDate.Date,
                Hour = request.FromDate.Hour,
                Minute = request.FromDate.Minute,
                Type = request.RequestType,
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
            ViewBag.Shifts = ShiftService.GetAll(new GetAllShiftCriteria());
            ViewBag.Updated = RequestService.UpdateCheckInOut(ModelState, model);
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
                Role = (short)RolesEnum.Member,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                EmployeeId = this.CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null
            };

            IList<CheckInOut> entityList = EmployeeService.GetCheckInOutRequest(checkInOutRequestCriteria);

            return View(entityList);
        }
    }
}
