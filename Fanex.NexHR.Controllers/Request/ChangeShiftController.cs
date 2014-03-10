// --------------------------------------------------------------------------
// <copyright file="ChangeShiftController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Created</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Change shift controller.
    /// </summary>
    [Authorize]
    public class ChangeShiftController : RequestController
    {
        #region Ctor
        public ChangeShiftController()
            : base(null, null, null, null)
        {
        }
        #endregion

        /// <summary>
        /// Create change shift request.
        /// </summary>
        /// <returns>Action result.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", 
            "SP2101:MethodMustNotContainMoreLinesThan", 
            Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult Create()
        {
            var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = this.CurrentUser.Id,
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now.AddMonths(1)
            };

            ViewBag.UnavailDates = EmployeeService.GetDisableRequestDate(getUnavailableRequestDateCriteria);
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = this.CurrentUser.Id };
            var employee = EmployeeService.Get(getEmployeeCriteria);
            var getShiftByDateCriteria = new GetShiftByDateCriteria()
            {
                EmployeeId = employee.Id,
                Date = DateTime.Now.Date
            };
            var shift = ShiftService.GetByDate(getShiftByDateCriteria);
            var model = new ChangeShiftViewModel
            {
                Employee = employee.NickName,
                EmployeeId = employee.Id,
                ForDate = DateTime.Now.Date,
                DisplayName = employee.DisplayName,
                By = this.CurrentUser.Id
            };
            if (shift != null)
            {
                model.FromShift = (int)shift.Id;
                model.FromShiftName = shift.Name;
            }

            // Count number of ChangeShift request in current month.
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int numberOfRequest = 0;
            var getChangeShiftRequestCriteria = new GetChangeShiftRequestCriteria
            {
                Role = (short)RolesEnum.Member,
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                EmployeeId = this.CurrentUser.Id,
                Status = null,
                GroupId = null,
                ExcludeEmployeeId = null
            };

            IList<ChangingShift> entityList = EmployeeService.GetChangeShiftRequest(getChangeShiftRequestCriteria);
            foreach (ChangingShift request in entityList)
            {
                if (request.Status.Id != (byte)RequestStatuses.Reject)
                {
                    numberOfRequest++;
                }
            }

            ViewBag.TotalRequest = numberOfRequest;
            ViewBag.Shifts = ShiftService.GetAll(new GetAllShiftCriteria());

            return View(model);
        }

        /// <summary>
        /// Create change shift request.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Create(ChangeShiftViewModel model)
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

                ViewBag.Requested = RequestService.ChangeShift(ModelState, model);
            }

            return View(model);
        }

        /// <summary>
        /// Update change shift request.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <returns>Action result.</returns>
        public ActionResult Update(int id)
        {
            var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = this.CurrentUser.Id,
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now.AddMonths(1)
            };

            ViewBag.UnavailDates = EmployeeService.GetDisableRequestDate(getUnavailableRequestDateCriteria);
            var request = EmployeeService.GetWorkingTimeRequest(id);
            var getShiftByDateCriteria = new GetShiftByDateCriteria()
            {
                EmployeeId = this.CurrentUser.Id,
                Date = request.FromDate.Date
            };
            var shift = ShiftService.GetByDate(getShiftByDateCriteria);
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = request.EmpId };
            var employee = EmployeeService.Get(getEmployeeCriteria);
            var model = new ChangeShiftViewModel
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                Id = request.Id,
                ForDate = request.FromDate.Date,
                FromShift = (int)shift.Id,
                FromShiftName = shift.Name,
                ToShift = (int)request.ToShift,
                IsPlanned = request.IsPlanned,
                Remark = request.Remark,
                By = this.CurrentUser.Id
            };

            ViewBag.Shifts = ShiftService.GetAll(new GetAllShiftCriteria());
            return View(model);
        }

        /// <summary>
        /// Update change shift request.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Update(ChangeShiftViewModel model)
        {
            model.By = this.CurrentUser.Id;
            ViewBag.Shifts = ShiftService.GetAll(new GetAllShiftCriteria());
            ViewBag.Updated = RequestService.UpdateChangeShift(ModelState, model);
            return View(model);
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
        /// Employee view.
        /// </summary>
        /// <param name="year">The year value.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;
            var getChangeShiftRequestCriteria = new GetChangeShiftRequestCriteria
            {
                Role = (short)RolesEnum.Member,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                EmployeeId = this.CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null
            };

            IList<ChangingShift> entityList = EmployeeService.GetChangeShiftRequest(getChangeShiftRequestCriteria);

            return View(entityList);
        }
    }
}
