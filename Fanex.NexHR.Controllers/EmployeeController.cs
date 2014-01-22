// --------------------------------------------------------------------------
// <copyright file="EmployeeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.7.3">Created</change>
//   <change who="Anders.Le" date="2012.7.3">Modify avatar function</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;
    using Simplicode.Imaging;
    using Simplicode.Imaging.Filters;

    /// <summary>
    /// Employee controller.
    /// </summary>
    [Authorize]
    public class EmployeeController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly ILookupDataService _lookupDataService;
        private readonly IShiftService _shiftService;
        private readonly IEmployeeService _employeeService;
        private readonly ITOEICService _toeicService;

        public EmployeeController()
            : this(null, null, null, null, null)
        {
        }

        public EmployeeController(
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
            ViewBag.Groups = _teamService.GetAll();
            return View();
        }

        [Authorize(Roles = "HR")]
        public ActionResult UserManagement(string jobTitleId = "")
        {
            ViewBag.JobTitleId = jobTitleId;
            ViewBag.Groups = _teamService.GetAll();

            var listJobTitle = _lookupDataService.GetAll(LookupCodeName.JobTitle);
            ViewBag.JobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = j.Id.ToString() == jobTitleId
            }).ToList();

            return View();
        }

        public ActionResult Detail(int id = -1)
        {
            var model = _employeeService.Get(id);

            if (model != null)
            {
                ViewBag.JobTitle = (model.JobTitleId == null)
                ? Resources.Labels.Unknown
                : _lookupDataService.Get(LookupCodeName.JobTitle, model.JobTitleId.Value).Name;

                ViewBag.Working = _employeeService.GetMySpace(id);
                var workingCalendar = _employeeService.GetEmployeeShift(id);
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
            }

            // TOEIC score
            ViewBag.TOEIC = _toeicService.GetById(id);

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            ViewBag.Shifts = _shiftService.GetSelectList();

            var teamList = _teamService.GetAll();
            ViewBag.TeamList = teamList;

            return View(new CreateEmployeeViewModel
            {
                MonShift = 1,
                TueShift = 1,
                WedShift = 1,
                ThuShift = 1,
                FriShift = 1,
            });
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(CreateEmployeeViewModel model)
        {
            model.ProcessBy = this.CurrentUser.Id;
            ViewBag.Shifts = _shiftService.GetSelectList();

            var teamList = _teamService.GetAll();
            ViewBag.TeamList = teamList;

            if (ModelState.IsValid)
            {
                model.DomainUsername = ConfigurationManager.AppSettings["domain"] + model.DomainUsername;
                var id = _employeeService.Insert(model);
                if (id == -1)
                {
                    ModelState.AddModelError(string.Empty, Resources.Messages.DuplicateEmployeeCode);
                }
                else
                {
                    return RedirectToAction("Edit", new { id = id });
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id = -1)
        {
            var model = _employeeService.Get(id);

            if (model != null)
            {
                // sub teams
                var subTeams = _teamService.GetSubTeamsOfEmployee(id);
                var subGroups = string.Empty;
                if (subTeams != null && subTeams.Count > 0)
                {
                    foreach (var team in subTeams)
                    {
                        subGroups += team.Id + ",";
                    }

                    subGroups = subGroups.Substring(0, subGroups.Length - 1);
                }

                model.SubGroups = subGroups;

                // teams handle requests
                var teamsHandleRequests = _teamService.GetTeamsHandleRequestsOfEmployee(id);
                var leaderGroups = string.Empty;
                if (teamsHandleRequests != null && teamsHandleRequests.Count > 0)
                {
                    foreach (var team in teamsHandleRequests)
                    {
                        leaderGroups += team.Id + ",";
                    }

                    leaderGroups = leaderGroups.Substring(0, leaderGroups.Length - 1);
                }

                model.LeaderGroups = leaderGroups;

                // get TOIEC score
                model.TOEICScore = _toeicService.GetById(id);

                ViewBag.Groups = _teamService.GetAll();
                ViewBag.JobTitles = _lookupDataService.GetAll(LookupCodeName.JobTitle);
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee model)
        {
            model.Modifier = this.CurrentUser;
            ViewBag.Groups = _teamService.GetAll();
            ViewBag.JobTitles = _lookupDataService.GetAll(LookupCodeName.JobTitle);

            if (ModelState.IsValid)
            {
                if (model.SubGroupArray != null && model.SubGroupArray.Contains(model.Team.Id.ToString()))
                {
                    model.SubGroupArray = model.SubGroupArray.Where(i => i != model.Team.Id.ToString()).ToArray();
                }

                var result = _employeeService.Update(model);
                ViewBag.Updated = result;

                // update current user?
                if (result && this.CurrentUser.Id == model.Id)
                {
                    var currentUser = _employeeService.Get(model.Id);
                    HttpContext.Session["CurrentUser"] = currentUser;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult DefaultSchedule(int id = -1)
        {
            var model = _employeeService.GetEmployeeShift(id);

            if (model != null)
            {
                ViewBag.Employee = _employeeService.Get(id);
                ViewBag.Shifts = _shiftService.GetSelectList();
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult DefaultSchedule(EmployeeShift model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Updated = _employeeService.UpdateEmployeeShift(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            ViewBag.Employee = _employeeService.Get(model.EmpID);
            ViewBag.Shifts = _shiftService.GetSelectList();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult MonthSchedule(int id = -1)
        {
            ViewBag.Id = id;
            ViewBag.Employee = _employeeService.Get(id);
            ViewBag.Shifts = _shiftService.GetSelectList();

            return View();
        }

        [AllowAnonymous]
        public FileResult Avatar(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = this.CurrentUser.Code + ".jpg";
            }
            else
            {
                id += ".jpg";
            }

            if (System.IO.File.Exists(Server.MapPath("/avatars/" + id)))
            {
                return File("/avatars/" + id, "image/jpeg");
            }

            return File("/avatars/avatar.jpg", "image/jpeg");
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult UploadAvatar(string code, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var stream = new MemoryStream();
                var processor = new ImageProcessor();
                var resizer = new ResizeFilter(64, 64, ResizeMethod.Crop, AnchorLocation.Middle);

                processor.AddFilter(resizer);
                processor.OutputFormat = OutputFormat.Jpeg;
                processor.JpegCompression = 100;
                processor.ProcessImage(file.InputStream, stream);

                var image = Image.FromStream(stream);
                image.Save(Server.MapPath("/avatars/") + code + ".jpg");

                return Json(true);
            }

            return Json(null);
        }

        public ActionResult MySpace()
        {
            var model = _employeeService.GetMySpace(this.CurrentUser.Id);
            return View(model);
        }

        public ActionResult MyWorkingSchedule()
        {
            return View(_employeeService.GetWorkingSchedule(DateTime.Now, this.CurrentUser.Id));
        }

        public ActionResult WorkingCalendars(int month = 0, int year = 0)
        {
            var currentDate = new System.Text.StringBuilder();

            ViewBag.Year = year != 0 ? year : DateTime.Now.Year;
            ViewBag.Month = month != 0 ? month : DateTime.Now.Month;

            currentDate.AppendFormat("{0}/{1}/1", ViewBag.Year, ViewBag.Month);
            ViewBag.CurrentDate = currentDate;

            var userChangedShift = _employeeService.GetUserChangedShift(new DateTime(ViewBag.Year, ViewBag.Month, 1));

            ViewBag.UserChangeShift = userChangedShift;
            ViewBag.HalfDayOffs = _employeeService.GetHalfDayOffs(DateTime.Parse(currentDate.ToString()));

            return View(_employeeService.GetWorkingCalendars(currentDate.ToString()));
        }

        [HttpGet]
        [Authorize(Roles = "HR")]
        public ActionResult WorkingCalendarsEdit()
        {
            var model = new ChangeWorkingCalendarsViewModel
            {
                Date = DateTime.Now,
                IsOfDay = 0,
                ModifiedDate = DateTime.Now,
                ModifiedBy = this.CurrentUser.Id
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult WorkingCalendarsEdit(ChangeWorkingCalendarsViewModel model)
        {
            model.ModifiedDate = DateTime.Now;
            model.ModifiedBy = this.CurrentUser.Id;
            ViewBag.Requested = _employeeService.UpdateOffDayForAllEmployees(ModelState, model);

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ManagementDashboard()
        {
            if (this.CurrentUser.IsInRoles(RolesEnum.AssistantLeader))
            {
                var model = _employeeService.GetHrLeaderDashboard(DateTime.Today, this.CurrentUser.Id);
                return PartialView(model);
            }

            return Content(string.Empty);
        }

        [Authorize(Roles = "HR")]
        public ActionResult TeamManagement()
        {
            IList<Team> teams = _teamService.GetAll();

            return View(teams);
        }

        [Authorize(Roles = "HR")]
        public ActionResult AddTeam(int teamId = -1)
        {
            TeamViewModel model = null;

            // create new
            if (teamId == -1)
            {
                model = new TeamViewModel
                {
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id
                };
            }
            else
            {
                var team = _teamService.Get(teamId);
                if (team != null)
                {
                    model = new TeamViewModel
                    {
                        Id = teamId,
                        TeamCode = team.Code,
                        TeamName = team.Name,
                        AssistantLeaderId = team.AssistantLeader.Id,
                        AssistantLeader = team.AssistantLeader.DisplayName,
                        LeaderId = team.Leader.Id,
                        Leader = team.Leader.DisplayName
                    };
                }
                else
                {
                    // throw exception
                }
            }

            return PartialView(model);
        }

        #region TOEIC

        [HttpGet]
        public ActionResult TOEICManagement(string jobTitleId = "")
        {
            ViewBag.JobTitleId = jobTitleId;
            ViewBag.Groups = _teamService.GetAll();

            var listJobTitle = _lookupDataService.GetAll(LookupCodeName.JobTitle);
            ViewBag.JobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = j.Id.ToString() == jobTitleId
            }).ToList();

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateFullTOEIC(int? id, string name)
        {
            TOEIC toeic = null;

            if (id.HasValue && id.Value > 0)
            {
                toeic = _toeicService.GetById(id.Value);
                toeic = toeic ?? new TOEIC { EmpId = id.Value };
            }

            ViewBag.EmployeeName = name;

            return PartialView(toeic);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateFullTOEIC(TOEIC model)
        {
            int effectedRow = -1;
            if (ModelState.IsValid)
            {
                ValidateTOEIC(model);

                if (ModelState.IsValid)
                {
                    model.CreatedBy = model.ModifiedBy = CurrentUser.Id;
                    model.CreatedDate = model.ModifiedDate = DateTime.Now;

                    effectedRow = _toeicService.Update(model);
                }
            }

            return Json(new { code = effectedRow, error = ModelState.Values.SelectMany(v => v.Errors) }, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateTOEIC()
        {
            return PartialView(new TOEICViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateTOEIC(TOEICViewModel model)
        {
            int effectedRow = -1;
            if (ModelState.IsValid)
            {
                // convert to TOEIC
                DateTime currentDate = DateTime.Now;
                TOEIC toeic = new TOEIC
                {
                    EmpId = model.EmpId,
                    InputDate = model.InputDate,
                    LRExamDate = model.LRExamDate,
                    Listening = model.Listening,
                    Reading = model.Reading,
                    SWExamDate = model.SWExamDate,
                    Speaking = model.Speaking,
                    Writing = model.Writing,
                    CreatedBy = CurrentUser.Id,
                    CreatedDate = currentDate,
                    ModifiedBy = CurrentUser.Id,
                    ModifiedDate = currentDate
                };

                ValidateTOEIC(toeic);

                if (ModelState.IsValid)
                {
                    effectedRow = _toeicService.NewUpdate(toeic);
                }
            }

            return Json(new { code = effectedRow, error = ModelState.Values.SelectMany(v => v.Errors) }, JsonRequestBehavior.DenyGet);
        }

        private void ValidateTOEIC(TOEIC model)
        {
            if (model.EmpId <= 0)
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidEmployeeID);
            }

            // check date
            DateTime currentDate = DateTime.Today;

            if (model.LRExamDate.HasValue && model.LRExamDate.Value > currentDate)
            {
                ModelState.AddModelError(
                    string.Empty,
                    string.Format(Resources.Messages.InvalidTOEICLRExamDate, currentDate.ToString("dd MMM, yyyy")));
            }

            if (model.SWExamDate.HasValue && model.SWExamDate.Value > currentDate)
            {
                ModelState.AddModelError(
                    string.Empty,
                    string.Format(Resources.Messages.InvalidTOEICSWExamDate, currentDate.ToString("dd MMM, yyyy")));
            }

            // check L/R/S/W
            if ((!model.Listening.HasValue || model.Listening == 0) &&
                (!model.Reading.HasValue || model.Reading == 0) &&
                (!model.Speaking.HasValue || model.Speaking == 0) &&
                (!model.Writing.HasValue || model.Writing == 0))
            {
                if (model.LRExamDate.HasValue || model.SWExamDate.HasValue || !model.InputDate.HasValue)
                {
                    ModelState.AddModelError(string.Empty, Resources.Messages.EmptyLRAndSW);
                }
            }
            else
            {
                if (!ValidateNumberPair(model.Listening, model.Reading))
                {
                    ModelState.AddModelError(string.Empty, Resources.Messages.EmptyLRPair);
                }
                if (!ValidateNumberPair(model.Speaking, model.Writing))
                {
                    ModelState.AddModelError(string.Empty, Resources.Messages.EmptySWPair);
                }
            }
        }

        private bool ValidateNumberPair(int? number1, int? number2)
        {
            bool isValid = false;
            if ((number1.HasValue && number1.Value > 0) &&
                (number2.HasValue && number2.Value > 0))
            {
                isValid = true;
            }
            else if ((!number1.HasValue || number1.Value == 0) &&
                (!number2.HasValue || number2.Value == 0))
            {
                isValid = true;
            }

            return isValid;
        }

        #endregion
    }
}
