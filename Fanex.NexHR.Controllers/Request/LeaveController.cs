// --------------------------------------------------------------------------
// <copyright file="LeaveController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Leave controller.
    /// </summary>
    [Authorize]
    public class LeaveController : RequestController
    {
        #region Ctor
        public LeaveController()
            : base(null, null, null, null)
        {
        }
        #endregion

        /// <summary>
        /// Create leave request.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Create()
        {
            var employee = _employeeService.Get(this.CurrentUser.Id);

            var listUnavailDates = _employeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;
            ViewBag.LeaveTypes = _leaveTypeService.GetAll();
            var model = new LeaveRequestViewModel
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1,
                By = this.CurrentUser.Id
            };

            // Count number days of Leave request in current month.
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            decimal numberDaysOfRequest = 0;
            IList<Leave> entityList = _employeeService.GetLeaveRequest(
                (short)RolesEnum.Member,
                new DateTime(year, month, 1),
                new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                this.CurrentUser.Id);

            foreach (Leave request in entityList)
            {
                if (request.Status.Id != (byte)RequestStatuses.Reject)
                {
                    numberDaysOfRequest += request.TotalDays;
                }
            }

            ViewBag.TotalRequest = numberDaysOfRequest;
            return View(model);
        }

        /// <summary>
        /// Create leave request.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Create(LeaveRequestViewModel model)
        {
            ViewBag.LeaveTypes = _leaveTypeService.GetAll();

            if (model.EmployeeId <= 0)
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidEmployeeID);
            }
            else
            {
                model.By = this.CurrentUser.Id;
                ViewBag.Year = model.FromDate.Year;
                if (string.Compare(model.Employee, model.DisplayName, true) != 0)
                {
                    model.DisplayName = model.Employee;
                }

                ViewBag.Requested = _requestService.Leave(ModelState, model);
            }

            return View(model);
        }

        /// <summary>
        /// Update leave request.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <returns>Action result.</returns>
        public ActionResult Update(int id)
        {
            ViewBag.UnavailDates = _employeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));

            var request = _employeeService.GetWorkingTimeRequest(id);
            var employee = _employeeService.Get(request.EmpId);
            var model = new LeaveRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                FromDate = request.FromDate.Date,
                FromTime = Utilities.GetTime(request.FromDate),
                ToDate = request.ToDate.Value.Date,
                ToTime = Utilities.GetTime(request.ToDate),
                TotalDays = (decimal)request.TotalDays,
                LeaveTypeId = (int)request.LeaveTypeID,
                IsPlanned = request.IsPlanned,
                Remark = request.Remark,
                By = this.CurrentUser.Id,
                HalfDayOff = request.TotalDays == (decimal)0.5
            };

            ViewBag.LeaveTypes = _leaveTypeService.GetAll();

            return View(model);
        }

        /// <summary>
        /// Update leave request.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Update(LeaveRequestViewModel model)
        {
            ViewBag.LeaveTypes = _leaveTypeService.GetAll();
            ViewBag.Updated = _requestService.UpdateLeave(ModelState, model);
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
            //// kos push
            IList<Leave> entityList = _employeeService.GetLeaveRequest(
                (short)RolesEnum.Member,
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                this.CurrentUser.Id);

            return View(entityList);
        }
    }
}
