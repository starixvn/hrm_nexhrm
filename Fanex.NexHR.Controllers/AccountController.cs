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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed.")]
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly IShiftService _shiftService;
        private readonly ILookupDataService _lookupDataService;

        private readonly IEmployeeService _employeeService;
		private readonly ITOEICService _toeicService;

        public AccountController()
            : this(null, null, null, null, null)
        { 
        }

        public AccountController(
            IEmployeeService employeeService = null,
            ITeamService teamService = null,
            ILookupDataService lookupDataService = null,
            IShiftService shiftService = null,
			ITOEICService toeicService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _shiftService = shiftService ?? new ShiftService();
			_toeicService = toeicService ?? new TOEICService();
        }

        public ActionResult Index()
        {
            var model = this.CurrentUser;
			int empId = this.CurrentUser.Id;

            ViewBag.JobTitle = model.JobTitleId == null
            ? Resources.Labels.Unknown
            : _lookupDataService.Get(LookupCodeName.JobTitle, model.JobTitleId.Value).Name;

			ViewBag.Working = _employeeService.GetMySpace(empId);
			var workingCalendar = _employeeService.GetEmployeeShift(empId);
            var shifts = _shiftService.GetAll();

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

			ViewBag.TOEIC = _toeicService.GetById(empId);
            return View(model);
        }
    }
}