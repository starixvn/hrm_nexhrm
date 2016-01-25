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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    [Authorize]
    public class LeaveController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly IRequestService _requestService;

        #region Ctor

        public LeaveController()
            : this(null, null, null)
        {
        }

        public LeaveController(
            IEmployeeService employeeService = null,
            ILeaveTypeService leaveTypeService = null,
            IRequestService requestService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _leaveTypeService = leaveTypeService ?? new LeaveTypeService();
            _requestService = requestService ?? new RequestService();
        }

        #endregion Ctor

        public ActionResult EmployeeView()
        {
            return View();
        }

        public ActionResult ListLeaveRequest(int year)
        {
            var criteria = new GetRequestCriteria()
            {
                Role = (short)Role.Member,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                EmployeeId = this.CurrentUser.Id,
                GroupId = null,
                Status = null,
                ExcludeEmployeeId = null,
                RequestType = Convert.ToString((int)RequestType.Leave),
                IsFilterByEffectedDate = true
            };

            IList<RequestEntity> leaveRequests = _requestService.GetRequest(criteria);

            return PartialView(leaveRequests);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult Create()
        {
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = this.CurrentUser.Id };
            var employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
            var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = this.CurrentUser.Id,
                FromDate = Utilities.RequestAvailableFromDate,
                ToDate = Utilities.RequestAvailableToDate
            };
            var model = new LeaveRequestViewModel
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1,
                By = this.CurrentUser.Id,
                LeaveTypes = _leaveTypeService.GetAll().ToList(),
                UnavailDates = _employeeService.GetDisableRequestDate(getUnavailableRequestDateCriteria).ToList()
            };

            // Count number days of Leave request in current month.
            int year = DateTime.Now.Year,
                month = DateTime.Now.Month;
            var criteria = new GetNumberRequestInMonthCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                RequestType = (int)RequestType.Leave
            };

            ViewBag.TotalRequest = (decimal)_requestService.GetNumberRequestInMonth(criteria);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(LeaveRequestViewModel model)
        {
            if (model.EmployeeId <= 0)
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidEmployeeID);
            }
            else
            {
                model.LeaveTypes = _leaveTypeService.GetAll().ToList();
                model.By = this.CurrentUser.Id;
                ViewBag.Year = model.FromDate.Year;

                if (string.Compare(model.Employee, model.DisplayName, StringComparison.Ordinal) != 0)
                {
                    model.DisplayName = model.Employee;
                }

                var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
                {
                    EmployeeId = model.EmployeeId,
                    FromDate = Utilities.RequestAvailableFromDate,
                    ToDate = Utilities.RequestAvailableToDate
                };

                model.UnavailDates = _employeeService.GetDisableRequestDate(getUnavailableRequestDateCriteria).ToList();
                ViewBag.Requested = _requestService.Leave(ModelState, model);
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
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = request.Requester.Id };
            var employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
            var model = new LeaveRequestViewModel
            {
                Id = request.Id,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                FromDate = request.FromDate.Date,
                FromTime = Utilities.GetTime(request.FromDate),
                ToDate = request.ToDate,
                ToTime = Utilities.GetTime(request.ToDate),
                TotalDays = (decimal)request.TotalDays,
                LeaveTypeId = (int)request.LeaveType.Id,
                IsPlanned = request.IsPlanned,
                Remark = request.Remark,
                By = this.CurrentUser.Id,
                HalfDayOff = request.TotalDays == (decimal)0.5,
                LeaveTypes = _leaveTypeService.GetAll().ToList(),
                UnavailDates = _employeeService.GetDisableRequestDate(getUnavailableRequestDateCriteria).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(LeaveRequestViewModel model)
        {
            model.LeaveTypes = _leaveTypeService.GetAll().ToList();
            ViewBag.Updated = _requestService.UpdateLeave(ModelState, model);

            return View(model);
        }
    }
}