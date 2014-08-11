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
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Account controller.
    /// </summary>
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IShiftService _shiftService;
        private readonly ILookupDataService _lookupDataService;
        private readonly IEmployeeService _employeeService;
        private readonly ITOEICService _toeicService;

        public AccountController()
            : this(null, null, null, null)
        {
        }

        public AccountController(
            IEmployeeService employeeService = null,
            ILookupDataService lookupDataService = null,
            IShiftService shiftService = null,
            ITOEICService toeicService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _shiftService = shiftService ?? new ShiftService();
            _toeicService = toeicService ?? new TOEICService();
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
            : _lookupDataService.Get(LookupCodeName.JobTitle, model.JobTitleId.Value).Name;

            var getMySpaceCriteria = new GetMySpaceCriteria()
            {
                EmpId = empId,
                Date = DateTime.Now.Date
            };
            ViewBag.Working = _employeeService.GetMySpace(getMySpaceCriteria);
            var employeeShiftCriteria = new GetEmployeeShift() { Id = empId };
            var workingCalendar = _employeeService.GetEmployeeShift(employeeShiftCriteria);
            var shifts = _shiftService.GetAll(new GetAllShiftCriteria());

            ViewBag.Sun = workingCalendar.SunShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.SunShift).Name;

            ViewBag.Mon = workingCalendar.MonShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.MonShift).Name;

            ViewBag.Tue = workingCalendar.TueShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.TueShift).Name;

            ViewBag.Wed = workingCalendar.WedShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.WedShift).Name;

            ViewBag.Thu = workingCalendar.ThuShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.ThuShift).Name;

            ViewBag.Fri = workingCalendar.FriShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.FriShift).Name;

            ViewBag.Sat = workingCalendar.SatShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.SatShift).Name;

            ViewBag.TOEIC = _toeicService.GetById(criteria);
            return View(model);
        }
    }
}