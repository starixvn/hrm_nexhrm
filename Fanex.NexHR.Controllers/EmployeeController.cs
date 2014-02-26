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
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Criteria.TOEIC;
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

        /// <summary>
        /// Index page.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Index()
        {
            ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());
            return View();
        }

        /// <summary>
        /// Manage user.
        /// </summary>
        /// <param name="jobTitleId">The job title ID.</param>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult UserManagement(string jobTitleId = "")
        {
            ViewBag.JobTitleId = jobTitleId;
            ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());

            var listJobTitle = _lookupDataService.GetAll(LookupCodeName.JobTitle);
            ViewBag.JobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = j.Id.ToString() == jobTitleId
            }).ToList();

            return View();
        }

        /// <summary>
        /// Employee detail.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <returns>Action result.</returns>
        public ActionResult Detail(int id = -1)
        {
            var model = _employeeService.Get(id);
            var criteria = new GetTOEICScoreByEmployeeId()
            {
                EmpId = id
            };

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

            ViewBag.TOEIC = _toeicService.GetById(criteria);

            return View(model);
        }

        /// <summary>
        /// Create employee.
        /// </summary>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            ViewBag.Shifts = _shiftService.GetSelectList();
            ViewBag.TeamList = _teamService.GetAll(new GetAllTeamCriteria());

            return View(new CreateEmployeeViewModel
            {
                MonShift = 1,
                TueShift = 1,
                WedShift = 1,
                ThuShift = 1,
                FriShift = 1,
            });
        }

        /// <summary>
        /// Create employee.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(CreateEmployeeViewModel model)
        {
            model.ProcessBy = this.CurrentUser.Id;

            ViewBag.Shifts = _shiftService.GetSelectList();
            ViewBag.TeamList = _teamService.GetAll(new GetAllTeamCriteria());

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

        /// <summary>
        /// Edit employee.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id = -1)
        {
            var model = _employeeService.Get(id);
            var employeeToeicCriteria = new GetTOEICScoreByEmployeeId()
            {
                EmpId = id
            };

            if (model != null)
            {
                var subTeamCriteria = new GetSubTeamOfEmployeeCriteria()
                {
                    EmpId = id
                };
                var subTeams = _teamService.GetSubTeamsOfEmployee(subTeamCriteria);
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
                var teamHandleRequestCriteria = new GetTeamHandleRequestOfEmployee()
                {
                    Id = id
                };
                var teamsHandleRequests = _teamService.GetTeamsHandleRequestsOfEmployee(teamHandleRequestCriteria);
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
                model.TOEICScore = _toeicService.GetById(employeeToeicCriteria);
                ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());
                ViewBag.JobTitles = _lookupDataService.GetAll(LookupCodeName.JobTitle);
            }

            return View(model);
        }

        /// <summary>
        /// Edit employee.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee model)
        {
            model.Modifier = this.CurrentUser;
            ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());
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

        /// <summary>
        /// Default schedule.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <returns>Action result.</returns>
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

        /// <summary>
        /// Default schedule.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
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

        /// <summary>
        /// Month schedule.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult MonthSchedule(int id = -1)
        {
            ViewBag.Id = id;
            ViewBag.Employee = _employeeService.Get(id);
            ViewBag.Shifts = _shiftService.GetSelectList();

            return View();
        }

        /// <summary>
        /// Employee avatar.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <returns>The file result.</returns>
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

        /// <summary>
        /// Upload avatar.
        /// </summary>
        /// <param name="code">The avatar code.</param>
        /// <param name="file">The avatar file.</param>
        /// <returns>The JSON result.</returns>
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

        public ActionResult MyDashboard()
        {
            var model = _employeeService.GetMySpace(this.CurrentUser.Id);
            ViewBag.HasBirthdayOrFirstWorkingday = false;
            return View("MySpace", model);
        }

        /// <summary>
        /// My space view.
        /// </summary>
        /// <param name="isRedirectFromLogin">Is redirect from login page.</param>
        /// <returns>Action result.</returns>
        public ActionResult MySpace(bool? isRedirectFromLogin = true)
        {
            var model = _employeeService.GetMySpace(this.CurrentUser.Id);
            IList<Employee> employeesBirthday = null;
            IList<Employee> employeesFirstWorkingday = null;

            var criteria = new GetBirthdayAndFirstWorkingdayCriteria
            {
                OnDate = DateTime.Now.Date
            };

            if (isRedirectFromLogin.HasValue && isRedirectFromLogin.Value)
            {
                _employeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria, ref employeesBirthday, ref employeesFirstWorkingday);
            }

            var hasBirthdayOrFirstWorkingday = (employeesBirthday != null && employeesBirthday.Any())
                || (employeesFirstWorkingday != null && employeesFirstWorkingday.Any());

            ViewBag.HasBirthdayOrFirstWorkingday = hasBirthdayOrFirstWorkingday;
            ViewBag.EmployeesBirthday = employeesBirthday;
            ViewBag.EmployeesFirstWorkingday = employeesFirstWorkingday;

            return View(model);
        }

        /// <summary>
        /// MY working schedule.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult MyWorkingSchedule()
        {
            return View(_employeeService.GetWorkingSchedule(DateTime.Now, this.CurrentUser.Id));
        }

        /// <summary>
        /// Working calendar.
        /// </summary>
        /// <param name="month">The month value.</param>
        /// <param name="year">The year value.</param>
        /// <returns>Action result.</returns>
        public ActionResult WorkingCalendars(int month = 0, int year = 0)
        {
            var currentDate = new StringBuilder();

            ViewBag.Year = year != 0 ? year : DateTime.Now.Year;
            ViewBag.Month = month != 0 ? month : DateTime.Now.Month;

            currentDate.AppendFormat("{0}/{1}/1", ViewBag.Year, ViewBag.Month);
            ViewBag.CurrentDate = currentDate;

            var userChangedShift = _employeeService.GetUserChangedShift(new DateTime(ViewBag.Year, ViewBag.Month, 1));

            ViewBag.UserChangeShift = userChangedShift;
            ViewBag.HalfDayOffs = _employeeService.GetHalfDayOffs(DateTime.Parse(currentDate.ToString()));

            return View(_employeeService.GetWorkingCalendars(currentDate.ToString()));
        }

        /// <summary>
        /// Edit working calendar.
        /// </summary>
        /// <returns>Action result.</returns>
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

        /// <summary>
        /// Edit working calendar.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult WorkingCalendarsEdit(ChangeWorkingCalendarsViewModel model)
        {
            model.ModifiedDate = DateTime.Now;
            model.ModifiedBy = this.CurrentUser.Id;
            ViewBag.Requested = _employeeService.UpdateOffDayForAllEmployees(ModelState, model);

            return View(model);
        }

        /// <summary>
        /// Manage dashboard.
        /// </summary>
        /// <returns>Action result.</returns>
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

        /// <summary>
        /// Manage team.
        /// </summary>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult TeamManagement()
        {
            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria());

            return View(teams);
        }

        /// <summary>
        /// Add new team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <returns>Action result.</returns>
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
                var criteria = new GetTeamByIdCriteria
                {
                    Id = teamId
                };
                var team = _teamService.Get(criteria);

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

        /// <summary>
        /// Manage TOEIC score.
        /// </summary>
        /// <param name="jobTitleId">The job title ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        public ActionResult TOEICManagement(string jobTitleId = "")
        {
            ViewBag.JobTitleId = jobTitleId;
            ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());

            var listJobTitle = _lookupDataService.GetAll(LookupCodeName.JobTitle);
            ViewBag.JobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = j.Id.ToString() == jobTitleId
            }).ToList();

            return View();
        }

        /// <summary>
        /// Update full TOEIC.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <param name="name">Employee name.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateFullTOEIC(int? id, string name)
        {
            TOEIC toeic = null;

            if (id.HasValue && id.Value > 0)
            {
                var criteria = new GetTOEICScoreByEmployeeId()
                {
                    EmpId = id.Value
                };
                toeic = _toeicService.GetById(criteria);
                toeic = toeic ?? new TOEIC { EmpId = id.Value };
            }

            ViewBag.EmployeeName = name;

            return PartialView(toeic);
        }

        /// <summary>
        /// Update full TOEIC.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
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

        /// <summary>
        /// Update TOEIC score.
        /// </summary>
        /// <returns>Action result.</returns>
        [HttpGet]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateTOEIC()
        {
            return PartialView(new TOEICViewModel());
        }

        /// <summary>
        /// Update TOEIC.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateTOEIC(TOEICViewModel model)
        {
            int effectedRow = -1;
            if (ModelState.IsValid)
            {
                // convert to TOEIC
                var currentDate = DateTime.Now;
                var toeic = new TOEIC
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

        /// <summary>
        /// Validate TOEIC score.
        /// </summary>
        /// <param name="model">The model object.</param>
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

        /// <summary>
        /// Validate number pair.
        /// </summary>
        /// <param name="number1">The first number.</param>
        /// <param name="number2">The second number.</param>
        /// <returns>The result.</returns>
        private bool ValidateNumberPair(int? number1, int? number2)
        {
            if ((number1.HasValue && number1.Value > 0) &&
                (number2.HasValue && number2.Value > 0))
            {
                return true;
            }
            else if ((!number1.HasValue || number1.Value == 0) &&
                (!number2.HasValue || number2.Value == 0))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
