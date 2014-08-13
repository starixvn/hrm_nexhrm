// --------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Criteria.Toeic;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// Account controller.
    /// </summary>
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly ILookupDataService _lookupDataService;
        private readonly IEmployeeService _employeeService;
        private readonly IToeicService _toeicService;

        public AccountController()
            : this(null, null, null)
        {
        }

        public AccountController(
            IEmployeeService employeeService = null,
            ILookupDataService lookupDataService = null,
            IToeicService toeicService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _toeicService = toeicService ?? new ToeicService();
        }

        /// <summary>
        /// Index page.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Index()
        {
            var model = this.CurrentUser;
            int empId = this.CurrentUser.Id;
            var criteria = new GetToeicScoreByEmployeeIdCriteria() { EmpId = empId };

            ViewBag.JobTitle = model.JobTitleId == null
            ? Resources.Labels.Unknown
            : _lookupDataService.GetById(LookupCodeName.JobTitle, model.JobTitleId.Value).Name;

            var getMySpaceCriteria = new GetMySpaceCriteria()
            {
                EmpId = empId,
                Date = DateTime.Now.Date
            };
            ViewBag.Working = _employeeService.GetMySpace(getMySpaceCriteria);
            var employeeShiftCriteria = new GetEmployeeShiftCriteria() { Id = empId };
            var workingCalendar = _employeeService.GetEmployeeShift(employeeShiftCriteria);

            ViewBag.Sun = workingCalendar.SunShiftName;
            ViewBag.Mon = workingCalendar.MonShiftName;
            ViewBag.Tue = workingCalendar.TueShiftName;
            ViewBag.Wed = workingCalendar.WedShiftName;
            ViewBag.Thu = workingCalendar.ThuShiftName;
            ViewBag.Fri = workingCalendar.FriShiftName;
            ViewBag.Sat = workingCalendar.SatShiftName;

            ViewBag.TOEIC = _toeicService.GetById(criteria);
            return View(model);
        }
    }
}