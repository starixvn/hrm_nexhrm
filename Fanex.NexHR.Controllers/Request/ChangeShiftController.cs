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
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
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
        public ActionResult Create()
        {
            ViewBag.UnavailDates = EmployeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            var employee = EmployeeService.Get(this.CurrentUser.Id);
            var shift = ShiftService.GetByDate(employee.Id, DateTime.Now.Date);
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
            IList<ChangingShift> entityList = EmployeeService.GetChangeShiftRequest(
                (short)RolesEnum.Member,
                new DateTime(year, month, 1),
                new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                this.CurrentUser.Id);

            foreach (ChangingShift request in entityList)
            {
                if (request.Status.Id != (byte)RequestStatuses.Reject)
                {
                    numberOfRequest++;
                }
            }

            ViewBag.TotalRequest = numberOfRequest;
            ViewBag.Shifts = ShiftService.GetAll();

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
            ViewBag.Shifts = ShiftService.GetAll();

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
            ViewBag.UnavailDates = EmployeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            var request = EmployeeService.GetWorkingTimeRequest(id);
            var shift = ShiftService.GetByDate(this.CurrentUser.Id, request.FromDate.Date);
            var employee = EmployeeService.Get(request.EmpId);
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

            ViewBag.Shifts = ShiftService.GetAll();
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
            ViewBag.Shifts = ShiftService.GetAll();
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

            IList<ChangingShift> entityList = EmployeeService.GetChangeShiftRequest(
                (short)RolesEnum.Member,
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                this.CurrentUser.Id);

            return View(entityList);
        }
    }
}
