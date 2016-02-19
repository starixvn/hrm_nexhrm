namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Employee;
    using FPS.Client;
    using Simplicode.Imaging;
    using Simplicode.Imaging.Filters;

    public class EmployeeController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly IJobTitleService _jobTitleService;
        private readonly IShiftService _shiftService;
        private readonly IEmployeeService _employeeService;
        private readonly IToeicService _toeicService;

        public EmployeeController()
            : this(null, null, null, null, null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService,
            ITeamService teamService,
            IJobTitleService jobTitleService,
            IShiftService shiftService,
            IToeicService toeicService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _jobTitleService = jobTitleService ?? new JobTitleService();
            _shiftService = shiftService ?? new ShiftService();
            _toeicService = toeicService ?? new ToeicService();
        }

        public ActionResult Index()
        {
            var model = new StaffInfoViewModel();
            model.Teams = GetTeam();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult UserManagement(int jobTitleId = 0)
        {
            var model = new UserManagementViewModel()
            {
                JobTitleId = jobTitleId,
                Teams = GetTeam(),
                JobTitles = GetJobTitle()
            };

            return View(model);
        }

        public ActionResult Detail(int employeeId = -1)
        {
            var criteria = new GetEmployeeByIdCriteria() 
            {
                EmployeeId = employeeId 
            };
            Employee model = _employeeService.GetEmployeeById(criteria);
            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            var model = new CreateEmployeeViewModel()
            {
                MonShift = 1,
                TueShift = 1,
                WedShift = 1,
                ThuShift = 1,
                FriShift = 1,
                Teams = GetTeam(),
                Shifts = GetShift()
            };

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(CreateEmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.DomainUsername = AppSetting.DomainName + model.DomainUsername;

                var criteria = new InsertEmployeeCriteria()
                {
                    Code = model.Code,
                    CreateBy = CurrentUser.Id,
                    DomainUsername = model.DomainUsername,
                    FriShift = model.FriShift,
                    FullName = model.FullName,
                    TeamId = model.TeamId,
                    MonShift = model.MonShift,
                    NickName = model.NickName,
                    TueShift = model.TueShift,
                    SatShift = model.SatShift,
                    SunShift = model.SunShift,
                    ThuShift = model.ThuShift,
                    WedShift = model.WedShift
                };

                CommandResult result = _employeeService.Insert(criteria);
                if (result.Code == 0)
                {
                    return RedirectToAction("Edit", new { id = criteria.Id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            model.Teams = GetTeam();
            model.Shifts = GetShift();
            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int employeeId = -1)
        {
            var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = employeeId };
            Employee employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

            EditEmployeeViewModel model = new EditEmployeeViewModel(employee) 
            {
                Teams = GetTeam(),
                JobTitles = GetJobTitle()
            };

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditEmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                UpdateEmployeeCriteria criteria = FillDataUpdateEmployeeCriteria(model);
                CommandResult result = _employeeService.Update(criteria);

                if (result.Code == 0)
                {
                    ViewBag.Updated = true;
                    if (CurrentUser.Id == model.Id)
                    {
                        var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = model.Id };
                        Employee currentUser = _employeeService.GetEmployeeById(getEmployeeCriteria);
                        HttpContext.Session["CurrentUser"] = currentUser;
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            model.Teams = GetTeam();
            model.JobTitles = GetJobTitle();
            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public CommandResult Delete(int employeeId, DateTime resignedDate)
        {
            var criteria = new DeleteEmployeeCriteria()
            {
                EmployeeId = employeeId,
                ResignedDate = resignedDate
            };
            CommandResult result = _employeeService.Delete(criteria);

            return result;
        }

        [Authorize(Roles = "HR")]
        public ActionResult DefaultSchedule(int employeeId = -1)
        {
            Employee employee = GetEmployeeById(employeeId);
            DefaultScheduleViewModel model = GetDefaultScheduleFromEmployee(employee);
            model.Shifts = GetShift();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult DefaultSchedule(DefaultScheduleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateScheduleDefaultCriteria()
                {
                    EmployeeId = model.EmployeeId,
                    
                    MonShift = model.MonShiftId,
                    TueShift = model.TueShiftId,
                    WedShift = model.WedShiftId,
                    ThuShift = model.ThuShiftId,
                    FriShift = model.FriShiftId,
                    SatShift = model.SatShiftId,
                    SunShift = model.SunShiftId
                };
                CommandResult result = _employeeService.UpdateEmployeeShift(criteria);

                if (result.Code == 0)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            model.Shifts = GetShift();
            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult MonthSchedule(int employeeId = -1)
        {
            Employee employee = GetEmployeeById(employeeId);
            MonthlyScheduleViewModel model = new MonthlyScheduleViewModel()
            { 
                EmployeeId = employee.Id,
                EmployeeNickName = employee.NickName,
                Shifts = GetShift()
            };

            return View(model);
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
            var employees = GetAllEmployees();
            var sortedEmployees = SortEmployeeBirthdayByMonth(employees);

            return View(sortedEmployees);
        }

        public ActionResult MyProfile()
        {
            return View(CurrentUser);
        }

        [Authorize(Roles = "AssistantLeader,HR")]
        public JsonResult GetMembersByManager(string keyword, int? groupId = null)
        {
            var role = CurrentUser.IsInRoles(Role.HR) ? Role.HR : Role.AssistantLeader;
            var getMemberByManager = new GetMemberByManagerCriteria
            {
                ManagerId = this.CurrentUser.Id,
                Role = (short)role,
                Keyword = keyword,
                GroupId = groupId,
                ExcludedManager = false
            };

            return Json(_employeeService.GetMembersByManager(getMemberByManager).Take(10), JsonRequestBehavior.AllowGet);
        }

        private UpdateEmployeeCriteria FillDataUpdateEmployeeCriteria(EditEmployeeViewModel model)
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
                EmployeeId = model.Id,
                IdCardNo = model.CardNo,
                IssuedDate = model.IssuedDate,
                IssuedPlace = model.IssuedPlace,
                JobTitleId = model.JobTitleId,
                JoinedDate = model.JoinedDate,
                TeamId = model.TeamId,
                Mariage = model.Mariage,
                ModifiedBy = CurrentUser.Id,
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
            var getAllEmployeeCriteria = new GetEmployeeCriteria()
            {
                Query = null,
                TeamId = null,
                JobTitleId = null,
                Enable = null,
                IsDelete = null,
                PageNumber = 1,
                PageSize = int.MaxValue,
                TotalRow = 2000
            };

            return _employeeService.Query(getAllEmployeeCriteria);
        }

        private IList<JobTitle> GetJobTitle()
        {
            var criteria = new GetJobTitleCriteria();

            IList<JobTitle> jobTitles = _jobTitleService.GetJobTitle(criteria);
            return jobTitles;
        }

        private IList<Team> GetTeam()
        {
            var criteria = new GetTeamCriteria();

            IList<Team> teams = _teamService.GetAll(criteria);
            return teams;
        }

        private IList<Shift> GetShift()
        {
            IList<Shift> shifts = _shiftService.GetAll(new GetAllShiftCriteria());
            return shifts;
        }

        private Employee GetEmployeeById(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria()
            {
                EmployeeId = employeeId
            };

            return _employeeService.GetEmployeeById(criteria);
        }

        private DefaultScheduleViewModel GetDefaultScheduleFromEmployee(Employee employee)
        {
            var defaultSchedule = new DefaultScheduleViewModel()
            {
                EmployeeId = employee.Id,
                EmployeeNickName = employee.NickName,
                MonShiftId = employee.DefaultSchedule.Monday.Id,
                TueShiftId = employee.DefaultSchedule.Tuesday.Id,
                WedShiftId = employee.DefaultSchedule.Wednesday.Id,
                ThuShiftId = employee.DefaultSchedule.Thursday.Id,
                FriShiftId = employee.DefaultSchedule.Friday.Id,
                SatShiftId = employee.DefaultSchedule.Saturday.Id,
                SunShiftId = employee.DefaultSchedule.Sunday.Id
            };

            return defaultSchedule;
        }
    }
}