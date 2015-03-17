// --------------------------------------------------------------------------
// <copyright file="EmployeeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.7.3">Created</change>
//   <change who="Anders.Le" date="2012.7.3">Modify avatar function</change>
//   <change who="Aaron.Cao" date="2015.3.16">Added birthday function and TOEIC send mail function</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Criteria.Toeic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using FPS.Client;
    using Simplicode.Imaging;
    using Simplicode.Imaging.Filters;

    /// <summary>
    /// Employee controller.
    /// </summary>
    [Authorize]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
    public class EmployeeController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly ILookupDataService _lookupDataService;
        private readonly IShiftService _shiftService;
        private readonly IEmployeeService _employeeService;
        private readonly IToeicService _toeicService;
        private readonly IEmailService _emailService;

        public EmployeeController()
            : this(null, null, null, null, null, null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService = null,
            ITeamService teamService = null,
            ILookupDataService lookupDataService = null,
            IShiftService shiftService = null,
            IToeicService toeicService = null,
            IEmailService emailService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _shiftService = shiftService ?? new ShiftService();
            _toeicService = toeicService ?? new ToeicService();
            _emailService = emailService ?? new EmailService();
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
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.JobTitle };
            var listJobTitle = _lookupDataService.GetAll(getAllLookupDataCriteria);
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
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = id };
            var model = _employeeService.GetEmployeeById(getEmployeeCriteria);
            var toeicScoreCriteria = new GetToeicScoreByEmployeeIdCriteria() { EmpId = id };

            if (model != null)
            {
                ViewBag.JobTitle = (model.JobTitleId == null)
                ? Resources.Labels.Unknown
                : _lookupDataService.GetById(LookupCodeName.JobTitle, model.JobTitleId.Value).Name;

                var getMySpaceCriteria = new GetMySpaceCriteria()
                {
                    EmpId = id,
                    Date = DateTime.Now.Date
                };
                ViewBag.Working = _employeeService.GetMySpace(getMySpaceCriteria);
                var employeeShiftCriteria = new GetEmployeeShiftCriteria() { Id = id };
                var workingCalendar = _employeeService.GetEmployeeShift(employeeShiftCriteria);

                ViewBag.Sun = workingCalendar.SunShiftName;
                ViewBag.Mon = workingCalendar.MonShiftName;
                ViewBag.Tue = workingCalendar.TueShiftName;
                ViewBag.Wed = workingCalendar.WedShiftName;
                ViewBag.Thu = workingCalendar.ThuShiftName;
                ViewBag.Fri = workingCalendar.FriShiftName;
                ViewBag.Sat = workingCalendar.SatShiftName;
            }

            ViewBag.TOEIC = _toeicService.GetById(toeicScoreCriteria);

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
                var insertEmployeeCriteria = new InsertEmployeeCriteria()
                {
                    Code = model.Code,
                    CreateBy = model.ProcessBy,
                    Createddate = DateTime.Now.Date,
                    DomainUsername = model.DomainUsername,
                    FriShift = model.FriShift,
                    FullName = model.FullName,
                    MainGroupId = model.TeamId,
                    MonShift = model.MonShift,
                    NickName = model.NickName,
                    TueShift = model.TueShift,
                    SatShift = model.SatShift,
                    SunShift = model.SunShift,
                    ThuShift = model.ThuShift,
                    WedShift = model.WedShift
                };
                var id = _employeeService.Insert(insertEmployeeCriteria);
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
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id = -1)
        {
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = id };
            var model = _employeeService.GetEmployeeById(getEmployeeCriteria);
            var employeeToeicCriteria = new GetToeicScoreByEmployeeIdCriteria()
            {
                EmpId = id
            };

            if (model != null)
            {
                var subTeamCriteria = new GetSubTeamOfEmployeeCriteria() { EmpId = id };
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
                model.ToeicScore = _toeicService.GetById(employeeToeicCriteria);
                ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());
                var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.JobTitle };
                ViewBag.JobTitles = _lookupDataService.GetAll(getAllLookupDataCriteria);
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
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult Edit(Employee model)
        {
            model.Modifier = this.CurrentUser;
            ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.JobTitle };
            ViewBag.JobTitles = _lookupDataService.GetAll(getAllLookupDataCriteria);

            if (ModelState.IsValid)
            {
                if (model.SubGroupArray != null && model.SubGroupArray.Contains(model.Team.Id.ToString()))
                {
                    model.SubGroupArray = model.SubGroupArray.Where(i => i != model.Team.Id.ToString()).ToArray();
                }

                var criteria = FillDataUpdateEmployeeCriteria(model);
                var result = _employeeService.Update(criteria);
                ViewBag.Updated = result;

                // update current user?
                if (result && this.CurrentUser.Id == model.Id)
                {
                    var getEmployeeCriteria = new GetEmployeeCriteria() { Id = model.Id };
                    var currentUser = _employeeService.GetEmployeeById(getEmployeeCriteria);
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
            var employeeShiftCriteria = new GetEmployeeShiftCriteria() { Id = id };
            var model = _employeeService.GetEmployeeShift(employeeShiftCriteria);

            if (model != null)
            {
                var getEmployeeCriteria = new GetEmployeeCriteria() { Id = id };
                ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
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
                var criteria = new UpdateEmployeeShiftCriteria()
                {
                    Id = model.EmpId,
                    FriShift = model.FriShiftId,
                    MonShift = model.MonShiftId,
                    SatShift = model.SatShiftId,
                    SunShift = model.SunShiftId,
                    ThuShift = model.ThuShiftId,
                    TueShift = model.TueShiftId,
                    WedShift = model.WedShiftId
                };
                ViewBag.Updated = _employeeService.UpdateEmployeeShift(criteria);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = model.EmpId };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
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
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = id };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
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

        ////public ActionResult MyDashboard()
        ////{
        ////    var model = _employeeService.GetMySpace(this.CurrentUser.Id);
        ////    ViewBag.HasBirthdayOrFirstWorkingday = false;
        ////    return View("MySpace", model);
        ////}

        /// <summary>
        /// My space view.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult MySpace()
        {
            var getMySpaceCriteria = new GetMySpaceCriteria()
            {
                EmpId = this.CurrentUser.Id,
                Date = DateTime.Now.Date
            };
            var model = _employeeService.GetMySpace(getMySpaceCriteria);

            return View(model);
        }

        /// <summary>
        /// MY working schedule.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult MyWorkingSchedule()
        {
            var workingScheduleCriteria = new GetWorkingScheduleCriteria
            {
                ReportDate = DateTime.Now,
                EmployeeId = this.CurrentUser.Id
            };
            var model = _employeeService.GetWorkingSchedule(workingScheduleCriteria);

            return View(model);
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

            var getUserChangedShiftCriteria = new GetUserChangedShiftCriteria
            {
                ReportDate = new DateTime(ViewBag.Year, ViewBag.Month, 1),
                EmployeeId = null,
                GroupId = null
            };
            var userChangedShift = _employeeService.GetUserChangedShift(getUserChangedShiftCriteria);

            ViewBag.UserChangeShift = userChangedShift;
            var workingCalendarCriteria = new GetWorkingCalendarCriteria() { ReportDate = Convert.ToDateTime(currentDate.ToString()) };
            var model = _employeeService.GetWorkingCalendars(workingCalendarCriteria);

            var getHalfDayOffCriteria = new GetHalfDayOffCriteria
            {
                ReportDate = DateTime.Parse(currentDate.ToString()),
                EmployeeId = null,
                GroupId = null
            };
            ViewBag.HalfDayOffs = _employeeService.GetHalfDayOffs(getHalfDayOffCriteria);
            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult WorkingCalendarInsert()
        {
            return PartialView(new InsertEmployeeCalendarViewModel
            {
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
            });
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
            var updateOffDayForAllEmployeesCriteria = new UpdateOffDayForAllEmployeesCriteria
            {
                Date = model.Date,
                IsOffDay = model.IsOfDay,
                ModifiedBy = model.ModifiedBy,
                ModifiedDate = model.ModifiedDate
            };

            ViewBag.Requested = _employeeService.UpdateOffDayForAllEmployees(ModelState, updateOffDayForAllEmployeesCriteria);

            return View(model);
        }

        /// <summary>
        /// Manage dashboard.
        /// </summary>
        /// <returns>Action result.</returns>
        [ChildActionOnly]
        public ActionResult ManagementDashboard()
        {
            if (this.CurrentUser.IsInRoles(Role.AssistantLeader))
            {
                var getHRLeaderDashboardCriteria = new GetHRLeaderDashboardCriteria()
                {
                    EmployeeId = this.CurrentUser.Id,
                    Date = DateTime.Today
                };
                var model = _employeeService.GetHRLeaderDashboard(getHRLeaderDashboardCriteria);
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
                Team team = _teamService.GetById(criteria);

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
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult PostFpsTransaction()
        {
            Service serviceFPS = new Service();
            serviceFPS.CheckTransaction(this.CurrentUser.NickName, "login", "SubTest2");

            return Content(string.Empty);
        }

        /// <summary>
        /// The birthday page which sorted by month.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Birthday()
        {
            var allEmployees = GetAllEmployees();
            var sortedEmployees = SortEmployeeBirthdayByMonth(allEmployees);

            return View(sortedEmployees);
        }

        #region TOEIC

        /// <summary>
        /// Manage TOEIC score.
        /// </summary>
        /// <param name="jobTitleId">The job title ID.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        public ActionResult ToeicManagement(string jobTitleId = "")
        {
            ViewBag.JobTitleId = jobTitleId;
            ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.JobTitle };
            var listJobTitle = _lookupDataService.GetAll(getAllLookupDataCriteria);
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
        public ActionResult UpdateFullToeic(int? id, string name)
        {
            Toeic toeic = null;

            if (id.HasValue && id.Value > 0)
            {
                var criteria = new GetToeicScoreByEmployeeIdCriteria()
                {
                    EmpId = id.Value
                };
                toeic = _toeicService.GetById(criteria);
                toeic = toeic ?? new Toeic { EmpId = id.Value };
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
        public ActionResult UpdateFullToeic(Toeic model)
        {
            int effectedRow = -1;
            var currentUserId = this.CurrentUser.Id;
            if (ModelState.IsValid)
            {
                ValidateTOEIC(model);

                if (ModelState.IsValid)
                {
                    var oldScore = GetToeicScore(model.EmpId);
                    var newScore = model;

                    model.CreatedBy = model.ModifiedBy = currentUserId;
                    model.CreatedDate = model.ModifiedDate = DateTime.Now;
                    var toeicCriteria = new UpdateFullToeicScoreCriteria()
                    {
                        CreatedBy = currentUserId, CreatedDate = DateTime.Now,
                        EmpId = model.EmpId, InputDate = model.InputDate,
                        Listening = model.Listening, LRExamDate = model.LRExamDate,
                        ModifiedBy = currentUserId, ModifiedDate = DateTime.Now,
                        OldListening = model.OldListening, OldReading = model.OldReading,
                        OldSpeaking = model.OldSpeaking, OldWriting = model.OldWriting,
                        Reading = model.Reading, Speaking = model.Speaking,
                        SWExamDate = model.SWExamDate, Writing = model.Writing
                    };
                    effectedRow = _toeicService.UpdateFullToeicScore(toeicCriteria);

                    if (effectedRow > 0)
                    {
                        SendUpdatedScoreEmailToRelatedPeople(model.EmpId, newScore, oldScore);
                    }
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
        public ActionResult UpdateToeic()
        {
            return PartialView(new ToeicViewModel());
        }

        /// <summary>
        /// Update TOEIC.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult UpdateToeic(ToeicViewModel model)
        {
            int effectedRow = -1;
            if (ModelState.IsValid)
            {
                // convert to TOEIC
                var currentDate = DateTime.Now;
                var toeic = new Toeic
                {
                    EmpId = model.EmpId, InputDate = model.InputDate,
                    LRExamDate = model.LRExamDate, Listening = model.Listening,
                    Reading = model.Reading, SWExamDate = model.SWExamDate,
                    Speaking = model.Speaking, Writing = model.Writing,
                    CreatedBy = CurrentUser.Id, CreatedDate = currentDate,
                    ModifiedBy = CurrentUser.Id, ModifiedDate = currentDate
                };

                ValidateTOEIC(toeic);

                if (ModelState.IsValid)
                {
                    var oldScore = GetToeicScore(model.EmpId);
                    var newScore = toeic;

                    var toeicScoreCriteria = new UpdateToeicScoreCriteria()
                    {
                        EmpId = model.EmpId, InputDate = model.InputDate,
                        LRExamDate = model.LRExamDate, Listening = model.Listening,
                        Reading = model.Reading, SWExamDate = model.SWExamDate,
                        Speaking = model.Speaking, Writing = model.Writing,
                        CreatedBy = CurrentUser.Id, CreatedDate = currentDate,
                        ModifiedBy = CurrentUser.Id, ModifiedDate = currentDate
                    };
                    effectedRow = _toeicService.UpdateToeicScore(toeicScoreCriteria);

                    if (effectedRow > 0)
                    {
                        SendUpdatedScoreEmailToRelatedPeople(model.EmpId, newScore, oldScore);
                    }
                }
            }

            return Json(new { code = effectedRow, error = ModelState.Values.SelectMany(v => v.Errors) }, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// Validate TOEIC score.
        /// </summary>
        /// <param name="model">The model object.</param>
        private void ValidateTOEIC(Toeic model)
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

        /// <summary>
        /// Fill Data Update Employee Criteria.
        /// </summary>
        /// <param name="model">Employee Model.</param>
        /// <returns>UpdateEmployeeCriteria Entity.</returns>
        private UpdateEmployeeCriteria FillDataUpdateEmployeeCriteria(Employee model)
        {
            var criteria = new UpdateEmployeeCriteria()
            {
                Code = model.Code,
                ConfirmedDate = model.ConfirmedDate,
                CurrAddress = model.CurrentAddress,
                Address = model.Address,
                Distancetocompany = model.DistanceToCompany,
                DateOfBirth = model.DateOfBirth,
                DomainUsername = model.DomainUsername,
                Email = model.Email,
                Enabled = model.Enabled,
                FullName = model.FullName,
                Gender = model.Gender,
                Geomaplatitude = model.GeoMapLatitude,
                Geomaplongtitude = model.GeoMapLongtitude,
                HandleRequestGroupIds = model.LeaderGroups,
                Id = model.Id,
                IdCardNo = model.CardNo,
                IssuedDate = model.IssuedDate,
                IssuedPlace = model.IssuedPlace,
                JobTitleId = model.JobTitleId,
                JoinedDate = model.JoinedDate,
                MainGroupId = model.Team.Id,
                Mariage = model.Mariage,
                ModifiedBy = model.Modifier.Id,
                ModifiedDate = DateTime.Now.Date,
                NickName = model.NickName,
                PassportExp = model.PassportExp,
                PassportNo = model.PassportNo,
                Phone = model.Phone,
                ResignedDate = model.ResignedDate,
                Roles = model.Roles,
                Skype = model.Skype,
                SubGroups = model.SubGroups,
                Yahoo = model.Yahoo,
                EmployeeCard = model.EmployeeCard
            };

            return criteria;
        }

        /// <summary>
        /// Send an email to the list of related people.
        /// </summary>
        /// <param name="empId">The employee ID updated score.</param>
        /// <param name="newScore">The new TOEIC score.</param>
        /// <param name="oldScore">The old TOEIC score.</param>
        private void SendUpdatedScoreEmailToRelatedPeople(int empId, Toeic newScore, Toeic oldScore)
        {
            var receiverEmail = string.Empty;
            var mailCC = string.Empty;
            var nickName = string.Empty;
            var subject = Labels.UpdateTOEICScore;

            if (newScore.Listening != oldScore.Listening || newScore.Reading != oldScore.Reading)
            {
                var criteria = new GetUpdatingScoreRelatedPeopleCriteria() { Id = empId };
                var employees = _employeeService.GetUpdatingScoreRelatedPeople(criteria);

                if (employees.Count() > 0)
                {
                    foreach (var employee in employees)
                    {
                        if (employee.Email.Length > 0 && employee.Id == empId)
                        {
                            receiverEmail = employee.Email;
                            nickName = employee.NickName;
                        }
                        else if (employee.Email.Length > 0)
                        {
                            mailCC += employee.Email + ";";
                        }
                    }

                    var body = Utilities.ReadTextFile(
                        System.Web.HttpContext.Current.Server.MapPath("~/app_data/emails/TOEICScoreUpdatedNotification.cshtml"));
                    var bodyContent = new
                    {
                        User = nickName,
                        NewListening = newScore.Listening.ToString(),
                        OldListening = oldScore.Listening != null ? oldScore.Listening.ToString() : "-",
                        NewReading = newScore.Reading.ToString(),
                        OldReading = oldScore.Reading != null ? oldScore.Reading.ToString() : "-"
                    };

                    body = string.Format(body, bodyContent.User, bodyContent.NewListening, bodyContent.OldListening, bodyContent.NewReading, bodyContent.OldReading);

                    this._emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        receiverEmail,
                        mailCC,
                        subject,
                        body);
                }
            }
        }

        /// <summary>
        /// Get the current TOEIC score.
        /// </summary>
        /// <param name="empid">The Employee ID.</param>
        /// <returns>The TOEIC score of employee.</returns>
        private Toeic GetToeicScore(int empid)
        {
            var criteria = new GetToeicScoreByEmployeeIdCriteria() { EmpId = empid };
            var score = _toeicService.GetById(criteria);
            if (score == null)
            {
                score = new Toeic();
            }

            return score;
        }

        #endregion

        #region BIRTHDAY

        /// <summary>
        /// Sort employee by birthday.
        /// </summary>
        /// <param name="employees">All current employees.</param>
        /// <returns>List of employees sorted by month.</returns>
        private IEnumerable<EmployeeBirthdayViewModel> SortEmployeeBirthdayByMonth(IEnumerable<Employee> employees)
        {
            var sortedEmployees = new List<EmployeeBirthdayViewModel>();

            if (employees.Count() > 0)
            {
                employees = employees.OrderBy(e => e.DateOfBirth.Month).ThenBy(e => e.DateOfBirth.Day);

                foreach (var employee in employees)
                {
                    var employeeBirthdayViewModel = new EmployeeBirthdayViewModel();
                    employeeBirthdayViewModel.DateOfBirth = employee.DateOfBirth.ToString("dd-MMM-yyyy");
                    employeeBirthdayViewModel.FullName = employee.FullName;
                    employeeBirthdayViewModel.Month = employee.DateOfBirth.Month;
                    employeeBirthdayViewModel.NickName = employee.NickName;

                    sortedEmployees.Add(employeeBirthdayViewModel);
                }
            }

            return sortedEmployees;
        }

        /// <summary>
        /// Get all employees.
        /// </summary>
        /// <returns>The information of all employees in company.</returns>
        private IEnumerable<Employee> GetAllEmployees()
        {
            var getAllEmployeeCriteria = new GetAllEmployeeCriteria()
            {
                Query = null,
                GroupId = null,
                TitleId = null,
                Enable = null,
                IsDelete = null,
                PageNumber = 1,
                PageSize = int.MaxValue,
                TotalRow = 2000
            };

            return _employeeService.Query(getAllEmployeeCriteria);
        }

        #endregion
    }
}
