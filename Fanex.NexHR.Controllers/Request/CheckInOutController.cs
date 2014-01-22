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
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [Authorize]
    public class CheckInOutController : RequestController
    {
        #region Ctor

        public CheckInOutController()
            : base(null, null, null, null)
        {
        }

        #endregion Ctor

        public ActionResult Create()
        {
            var listUnavailDates = _employeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;

            var employee = _employeeService.Get(this.CurrentUser.Id);
            ViewBag.Shifts = _shiftService.GetAll();

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

            var shift = _shiftService.GetByDate(employee.Id, DateTime.Now);
            if (shift != null)
            {
                viewModel.Hour = int.Parse(shift.BeginCheckInTime.Split(new[] { ':' })[0]);
                viewModel.Minute = int.Parse(shift.BeginCheckInTime.Split(new[] { ':' })[1]);
            }

            /*Anders check null shiftdate*/
            viewModel.Shift = (shift == null) ? string.Empty : shift.Name;

            // Count number of Check in/out request in current month.
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int numberOfRequest = 0;
            IList<CheckInOut> entityList = _employeeService.GetCheckInOutRequest(
                (short)RolesEnum.Member,
                new DateTime(year, month, 1),
                new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                this.CurrentUser.Id);

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

        [HttpPost]
        public ActionResult Create(CheckInOutRequestViewModel model)
        {
            ViewBag.Shifts = _shiftService.GetAll();

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

                ViewBag.Requested = _requestService.CheckInOut(ModelState, model);
                ViewBag.Year = model.ForDate.Year;
            }

            return View(model);
        }

        public ActionResult Update(int id)
        {
            var listUnavailDates = _employeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;

            ViewBag.Shifts = _shiftService.GetAll();

            var request = _employeeService.GetWorkingTimeRequest(id);

            if (!this.Editable(request.Status, request.EmpId, request.CreatedBy))
            {
                return RedirectToAction("EmployeeView", "CheckInOut");
            }

            var employee = _employeeService.Get(request.EmpId);
            var shift = _shiftService.GetByDate(employee.Id, DateTime.Now);
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

        [HttpPost]
        public ActionResult Update(CheckInOutRequestViewModel model)
        {
            model.By = this.CurrentUser.Id;
            ViewBag.Shifts = _shiftService.GetAll();
            ViewBag.Updated = _requestService.UpdateCheckInOut(ModelState, model);
            ViewBag.Year = model.ForDate.Year;
            return View(model);
        }

        public ActionResult Delete()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        /// <summary>
        /// Get check in/out request of a specific employee in a year.
        /// </summary>
        /// <param name="year">In Year.</param>
        /// <returns>Out View.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;

            IList<CheckInOut> entityList = _employeeService.GetCheckInOutRequest(
                (short)RolesEnum.Member,
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                this.CurrentUser.Id);

            return View(entityList);
        }
    }
}
