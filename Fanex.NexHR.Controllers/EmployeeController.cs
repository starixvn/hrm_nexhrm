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
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Criteria.Toeic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using FPS.Client;
    using Simplicode.Imaging;
    using Simplicode.Imaging.Filters;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
    public class EmployeeController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly ILookupDataService _lookupDataService;
        private readonly IShiftService _shiftService;
        private readonly IEmployeeService _employeeService;
        private readonly IToeicService _toeicService;

        public EmployeeController()
            : this(null, null, null, null, null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService = null,
            ITeamService teamService = null,
            ILookupDataService lookupDataService = null,
            IShiftService shiftService = null,
            IToeicService toeicService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _shiftService = shiftService ?? new ShiftService();
            _toeicService = toeicService ?? new ToeicService();
        }

        public ActionResult Index()
        {
            var model = new StaffInfoViewModel();
            model.TeamCollection = _teamService.GetAll(new GetAllTeamCriteria());

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult UserManagement(string jobTitleId = "")
        {
            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria());

            var getAllLookupDataCriteria = new GetAllLookupDataCriteria()
            {
                LookupCode = LookupCodeName.JobTitle
            };
            IList<LookupData> jobTitles = _lookupDataService.GetAll(getAllLookupDataCriteria);

            var model = new UserManagementViewModel()
            {
                JobTitleId = jobTitleId,
                TeamCollection = teams,
                JobTitleCollection = jobTitles
            };

            return View(model);
        }

        public ActionResult Detail(int id = -1)
        {
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = id };
            var model = _employeeService.GetEmployeeById(getEmployeeCriteria);
            var toeicScoreCriteria = new GetToeicScoreByEmployeeIdCriteria() { EmpId = id };

            if (model != null)
            {
                ViewBag.JobTitle = (model.JobTitleId == null)
                    ? Resources.Labels.Unknown : _lookupDataService.GetById(LookupCodeName.JobTitle, model.JobTitleId.Value).Name;

                var getMySpaceCriteria = new GetMySpaceCriteria()
                {
                    EmpId = id
                };
                ViewBag.Working = _employeeService.GetMySpace(getMySpaceCriteria);

                var employeeShiftCriteria = new GetEmployeeShiftCriteria() { Id = id };
                model.WorkingCalendar = _employeeService.GetEmployeeShift(employeeShiftCriteria);
            }

            ViewBag.TOEIC = _toeicService.GetById(toeicScoreCriteria);

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria());
            IList<SelectListItem> shifts = _shiftService.GetSelectList();

            var model = new CreateEmployeeViewModel()
            {
                MonShift = 1,
                TueShift = 1,
                WedShift = 1,
                ThuShift = 1,
                FriShift = 1,
                TeamCollection = teams,
                ShiftCollection = shifts
            };

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(CreateEmployeeViewModel model)
        {
            model.ProcessBy = this.CurrentUser.Id;
            model.ShiftCollection = _shiftService.GetSelectList();
            model.TeamCollection = _teamService.GetAll(new GetAllTeamCriteria());

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

                int id = _employeeService.Insert(insertEmployeeCriteria);
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

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id = -1)
        {
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = id };
            var employeeToeicCriteria = new GetToeicScoreByEmployeeIdCriteria()
            {
                EmpId = id
            };

            Employee model = _employeeService.GetEmployeeById(getEmployeeCriteria);

            if (model != null)
            {
                var subTeamCriteria = new GetSubTeamOfEmployeeCriteria() { EmpId = id };
                IList<Team> subTeams = _teamService.GetSubTeamsOfEmployee(subTeamCriteria);
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
                IList<Team> teamsHandleRequests = _teamService.GetTeamsHandleRequestsOfEmployee(teamHandleRequestCriteria);
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
                model.Groups = _teamService.GetAll(new GetAllTeamCriteria());

                var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.JobTitle };
                model.JobTitles = _lookupDataService.GetAll(getAllLookupDataCriteria);
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult Edit(Employee model)
        {
            model.Modifier = this.CurrentUser;
            model.Groups = _teamService.GetAll(new GetAllTeamCriteria());

            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.JobTitle };
            model.JobTitles = _lookupDataService.GetAll(getAllLookupDataCriteria);

            if (ModelState.IsValid)
            {
                if (model.SubGroupArray != null && model.SubGroupArray.Contains(model.Team.Id.ToString()))
                {
                    model.SubGroupArray = model.SubGroupArray.Where(i => i != model.Team.Id.ToString()).ToArray();
                }

                var criteria = FillDataUpdateEmployeeCriteria(model);
                bool result = _employeeService.Update(criteria);
                ViewBag.Updated = result;

                // update current user?
                if (result && this.CurrentUser.Id == model.Id)
                {
                    var getEmployeeCriteria = new GetEmployeeCriteria() { Id = model.Id };
                    Employee currentUser = _employeeService.GetEmployeeById(getEmployeeCriteria);
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
            var employeeShiftCriteria = new GetEmployeeShiftCriteria() { Id = id };
            EmployeeShift model = _employeeService.GetEmployeeShift(employeeShiftCriteria);

            if (model != null)
            {
                var getEmployeeCriteria = new GetEmployeeCriteria() { Id = id };
                ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
                model.Shifts = _shiftService.GetSelectList();
            }

            return View(model);
        }

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
            model.Shifts = _shiftService.GetSelectList();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult MonthSchedule(int id = -1)
        {
            ViewBag.Id = id;
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = id };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
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
            MySpaceViewModel myspace = GetMySpace();
            return View(myspace);
        }

        public ActionResult MySpace2()
        {
            var getMySpaceCriteria = new GetMySpaceCriteria()
            {
                EmpId = this.CurrentUser.Id
            };
            var model = _employeeService.GetMySpace(getMySpaceCriteria);

            return View(model);
        }

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

        [ChildActionOnly]
        public ActionResult ManagementDashboard()
        {
            if (CurrentUser.IsInRoles(Role.AssistantLeader))
            {
                var criteria = new GetHRLeaderDashboardCriteria()
                {
                    EmployeeId = CurrentUser.Id,
                };
                var model = _employeeService.GetHRLeaderDashboard(criteria);
                return PartialView(model);
            }

            return Content(string.Empty);
        }

        [HttpPost]
        public ActionResult PostFpsTransaction()
        {
            Service serviceFPS = new Service();
            var resultMessage = string.Empty;
            var deviceCode = string.Empty;
            serviceFPS.CheckTransaction(this.CurrentUser.NickName, "login", string.Empty, "SubTest2", out resultMessage, out deviceCode);

            return Content(string.Empty);
        }

        public ActionResult Birthday()
        {
            var allEmployees = GetAllEmployees();
            var sortedEmployees = SortEmployeeBirthdayByMonth(allEmployees);

            return View(sortedEmployees);
        }

        public ActionResult MyProfile()
        {
            var model = this.CurrentUser;
            var criteria = new GetToeicScoreByEmployeeIdCriteria() { EmpId = CurrentUser.Id };

            ViewBag.JobTitle = model.JobTitleId == null
                ? Resources.Labels.Unknown : _lookupDataService.GetById(LookupCodeName.JobTitle, model.JobTitleId.Value).Name;

            var getMySpaceCriteria = new GetMySpaceCriteria()
            {
                EmpId = CurrentUser.Id
            };
            ViewBag.Working = _employeeService.GetMySpace(getMySpaceCriteria);

            var employeeShiftCriteria = new GetEmployeeShiftCriteria() { Id = CurrentUser.Id };
            model.WorkingCalendar = _employeeService.GetEmployeeShift(employeeShiftCriteria);

            ViewBag.TOEIC = _toeicService.GetById(criteria);
            return View(model);
        }

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

        private MySpaceViewModel GetMySpace()
        {
            var criteria = new GetMySpaceCriteria()
            {
                EmpId = CurrentUser.Id
            };
            MySpaceViewModel myspace = _employeeService.GetMySpace(criteria);

            return myspace;
        }

        #region BIRTHDAY

        private IEnumerable<EmployeeBirthdayViewModel> SortEmployeeBirthdayByMonth(IEnumerable<Employee> employees)
        {
            var sortedEmployees = new List<EmployeeBirthdayViewModel>();

            if (employees.Any())
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

        #endregion BIRTHDAY
    }
}