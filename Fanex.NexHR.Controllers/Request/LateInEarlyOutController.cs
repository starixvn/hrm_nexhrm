// --------------------------------------------------------------------------
// <copyright file="LateInEarlyOutController.cs" company="Nexcel Solutions Vietnam">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1404:CodeAnalysisSuppressionMustHaveJustification", Justification = "Reviewed.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    [Authorize]
    public class LateInEarlyOutController : RequestController
    {
        #region Ctor

        public LateInEarlyOutController()
            : base(null, null, null, null)
        {
        }

        #endregion

        public ActionResult Create()
        {
            var listUnavailDates = _employeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;

            var employee = _employeeService.Get(this.CurrentUser.Id);

            //// Count number of Late in/early out request in current month.
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int numberOfRequest = 0;
            IList<LateInEarlyOut> entityList = _employeeService.GetLateInEarlyOutRequest(
                (short)RolesEnum.Member,
                new DateTime(year, month, 1),
                new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                this.CurrentUser.Id);

            foreach (LateInEarlyOut request in entityList)
            {
                if (request.Status.Id != (byte)RequestStatuses.Reject)
                {
                    numberOfRequest++;
                }
            }

            ViewBag.TotalRequest = numberOfRequest;

            return View(new LateInEarlyOutRequestViewModel
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                ForDate = DateTime.Now.Date,
                Minute = 60,
                Type = (short)RequestTypes.LateIn,
                By = this.CurrentUser.Id
            });
        }

        [HttpPost]
        public ActionResult Create(LateInEarlyOutRequestViewModel model)
        {
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

				ViewBag.Requested = _requestService.LateInEarlyOut(ModelState, model);
				ViewBag.Year = model.ForDate.Year;
			}

            return View(model);
        }

        public ActionResult Update(int id)
        {
            var listUnavailDates = _employeeService.GetDisableRequestDate(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;

            var request = _employeeService.GetWorkingTimeRequest(id);
            var employee = _employeeService.Get(request.EmpId);
            int minute = request.RequestType == (short)RequestTypes.LateIn
                ? (int)request.LateInMins
                : (int)request.EarlyOutMins;

            return View(new LateInEarlyOutRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                ForDate = request.FromDate.Date,
                Minute = minute,
                Type = request.RequestType,
                By = this.CurrentUser.Id,
                Remark = request.Remark
            });
        }

        [HttpPost]
        public ActionResult Update(LateInEarlyOutRequestViewModel model)
        {
            model.By = this.CurrentUser.Id;
            ViewBag.Updated = _requestService.UpdateLateInEarlyOut(ModelState, model);
            ViewBag.Year = model.ForDate.Year;
            return View(model);
        }

        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;

            IList<LateInEarlyOut> entityList = _employeeService.GetLateInEarlyOutRequest(
                (short)RolesEnum.Member,
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                this.CurrentUser.Id);

            return View(entityList);
        }
    }
}
