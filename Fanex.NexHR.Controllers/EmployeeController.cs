namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Core;
    using Core.Enums;
    using Core.Model;
    using Criteria.Employee;
    using Models;
    using Newtonsoft.Json;
    using Services;
    using Services.Interfaces;
    using Simplicode.Imaging;
    using ViewModels;
    using ViewModels.Employee;

    public class EmployeeController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly IJobTitleService _jobTitleService;
        private readonly IShiftService _shiftService;
        private readonly IEmployeeService _employeeService;
        private readonly ICommandService _commandService;

        public EmployeeController()
            : this(null, null, null, null, null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService,
            ITeamService teamService,
            IJobTitleService jobTitleService,
            IShiftService shiftService,
            ICommandService commandService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _jobTitleService = jobTitleService ?? new JobTitleService();
            _shiftService = shiftService ?? new ShiftService();
            _commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            var model = new StaffInfoViewModel { Teams = _teamService.GetTeam() };

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult UserManagement(int jobTitleId = 0)
        {
            var model = new UserManagementViewModel
            {
                JobTitleId = jobTitleId,
                Teams = _teamService.GetTeam(),
                JobTitles = _jobTitleService.GetAll()
            };

            return View(model);
        }

        [ValidateInput(false)]
        public JsonResult GetEmployees(
            string query,
            int? teamId = null,
            bool? enable = null,
            bool? isDelete = null,
            int? jobTitleId = null,
            string sort = null)
        {
            EmployeeListViewModel listEmployee =
                new EmployeeListViewModel
                {
                    StatusViewType = enable.HasValue
                        ? (enable.Value ? (int)TypeList.Disabled : (int)TypeList.Active)
                        : (int)TypeList.Deleted
                };

            var criteria = new GetEmployeeCriteria
            {
                Query = query,
                TeamId = teamId == -1 ? null : teamId,
                JobTitleId = jobTitleId,
                EnableStatus = enable,
                DeleteStatus = isDelete,
                PageNumber = 1,
                PageSize = int.MaxValue,
                TotalRow = 2000
            };

            listEmployee.Employees = _employeeService.Query(criteria);

            listEmployee.Employees = SortEmployee(sort, listEmployee.Employees);

            return Json(listEmployee, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int employeeId)
        {
            Employee employee = GetEmployeeById(employeeId);
            return View(employee);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            var model = new CreateEmployeeViewModel
            {
                MonShift = 1,
                TueShift = 1,
                WedShift = 1,
                ThuShift = 1,
                FriShift = 1,
                Teams = _teamService.GetAll(),
                Shifts = _shiftService.GetAll()
            };

            return View(model);
        }

        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(CreateEmployeeViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                model.DomainUsername = AppSetting.DomainName + model.DomainUsername;
                var criteria = new InsertEmployeeCriteria
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

                CommandResult result = _commandService.Insert(criteria, hasGetOutputParams: true);
                if (result.IsSuccessful)
                {
                    return Redirect("Edit?id=" + criteria.EmployeeId);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }

                model.Teams = _teamService.GetAll();
                model.Shifts = _shiftService.GetAll();
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            Employee employee = GetEmployeeById(id);

            EditEmployeeViewModel model = new EditEmployeeViewModel(employee)
            {
                Teams = _teamService.GetAll(),
                JobTitles = _jobTitleService.GetAll()
            };

            return View(model);
        }

        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditEmployeeViewModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return View(model);
            }

            UpdateEmployeeCriteria criteria = FillDataUpdateEmployeeCriteria(model);
            CommandResult result = _commandService.Update(criteria);

            if (result.IsSuccessful)
            {
                ViewBag.Updated = true;
                if (CurrentUser.Id == model.Id)
                {
                    var getEmployeeCriteria = new GetEmployeeByIdCriteria { EmployeeId = model.Id };
                    Employee currentUser = _employeeService.GetEmployeeById(getEmployeeCriteria);
                    if (HttpContext.Session != null)
                    {
                        HttpContext.Session["CurrentUser"] = currentUser;
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
            }

            model.Teams = _teamService.GetAll();
            model.JobTitles = _jobTitleService.GetAll();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int employeeId, DateTime resignedDate)
        {
            var criteria = new DeleteEmployeeCriteria
            {
                EmployeeId = employeeId,
                ResignedDate = resignedDate
            };
            CommandResult result = _commandService.Delete(criteria);

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        public ActionResult DefaultSchedule(int employeeId)
        {
            Employee employee = GetEmployeeById(employeeId);
            DefaultScheduleViewModel model = GetDefaultScheduleFromEmployee(employee);
            model.Shifts = _shiftService.GetAll();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult DefaultSchedule(DefaultScheduleViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateScheduleDefaultCriteria
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
                CommandResult result = _commandService.Update(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }

                model.Shifts = _shiftService.GetAll();
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult MonthSchedule(int employeeId)
        {
            Employee employee = GetEmployeeById(employeeId);
            MonthlyScheduleViewModel model = new MonthlyScheduleViewModel();
            if (employee != null)
            {
                model.EmployeeId = employee.Id;
                model.EmployeeNickName = employee.NickName;
            }

            model.Shifts = _shiftService.GetAll();
            return View(model);
        }

        [Authorize(Roles = "SpecialUser")]
        [AllowAnonymous]
        public FileResult Avatar(string id)
        {
            string fileName = "/avatars/avatar.jpg";
            string userId = id;

            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = CurrentUser.Code + ".jpg";
            }
            else
            {
                userId += ".jpg";
            }

            if (System.IO.File.Exists(Server.MapPath("/avatars/" + userId)))
            {
                fileName = "/avatars/" + userId;
            }

            return File(fileName, "image/jpeg");
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult UploadAvatar(string code, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                using (var stream = new MemoryStream())
                {
                    using (var processor = new ImageProcessor())
                    {
                        ////var resizer = new ResizeFilter(128, 128, ResizeMethod.Crop, AnchorLocation.Middle);
                        ////processor.AddFilter(resizer);
                        processor.OutputFormat = OutputFormat.Jpeg;
                        processor.JpegCompression = 100;
                        processor.ProcessImage(file.InputStream, stream);

                        using (var image = Image.FromStream(stream))
                        {
                            image.Save(Server.MapPath("/avatars/") + code + ".jpg");
                        }

                        return Json(true);
                    }
                }
            }

            return Json(null);
        }

        ////public ActionResult MySpace()
        ////{
        ////    var getMySpaceCriteria = new GetMySpaceCriteria()
        ////    {
        ////        EmpId = CurrentUser.Id
        ////    };
        ////    var model = employeeService.GetMySpace(getMySpaceCriteria);

        ////    return View(model);
        ////}

        ////[ChildActionOnly]
        ////public ActionResult ManagementDashboard()
        ////{
        ////    if (CurrentUser.IsInRoles(Role.AssistantLeader))
        ////    {
        ////        var criteria = new GetHRLeaderDashboardCriteria()
        ////        {
        ////            EmployeeId = CurrentUser.Id,
        ////        };
        ////        var model = employeeService.GetHRLeaderDashboard(criteria);
        ////        return PartialView(model);
        ////    }

        ////    return Content(string.Empty);
        ////}

        ////[HttpPost]
        ////public ActionResult PostFpsTransaction()
        ////{
        ////    using (var serviceFPS = new Service())
        ////    {
        ////        var resultMessage = string.Empty;
        ////        var deviceCode = string.Empty;
        ////        serviceFPS.CheckTransaction(CurrentUser.NickName, "login", string.Empty, "SubTest2", out resultMessage, out deviceCode);

        ////        return Content(string.Empty);
        ////    }
        ////}

        [Authorize(Roles = "SpecialUser")]
        public ActionResult Birthday(int month = 0)
        {
            IEnumerable<Employee> employees = GetEmployees();
            IList<EmployeeBirthdayViewModel> sortedEmployees;
            if (month != 0)
            {
                sortedEmployees = SortEmployeeBirthdayByMonth(employees.Where(e => e.DateOfBirth.Month == month));
                ViewBag.ShowOneMonth = true;
            }
            else
            {
                sortedEmployees = SortEmployeeBirthdayByMonth(employees);
                ViewBag.ShowOneMonth = false;
            }

            return View(sortedEmployees);
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult MyProfile()
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = CurrentUser.Id
            };
            Employee employee = _employeeService.GetEmployeeById(criteria);
            return View(employee);
        }

        [Authorize(Roles = "AssistantLeader,HR")]
        public JsonResult GetMembersByManager(string keyword, int? groupId = null)
        {
            var role = CurrentUser.IsInRoles(Role.HR) ? Role.HR : Role.AssistantLeader;
            var getMemberByManager = new GetMemberByManagerCriteria
            {
                ManagerId = CurrentUser.Id,
                Role = (short)role,
                Keyword = keyword,
                GroupId = groupId,
                ExcludedManager = false
            };

            return Json(_employeeService.GetMembersByManager(getMemberByManager).Take(10), JsonRequestBehavior.AllowGet);
        }

        private UpdateEmployeeCriteria FillDataUpdateEmployeeCriteria(EditEmployeeViewModel model)
        {
            var criteria = new UpdateEmployeeCriteria
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

        private static IList<EmployeeBirthdayViewModel> SortEmployeeBirthdayByMonth(IEnumerable<Employee> employees)
        {
            var sortedEmployees = new List<EmployeeBirthdayViewModel>();

            if (employees.Any())
            {
                var orderedEmployees = employees.OrderBy(e => e.DateOfBirth.Month).ThenBy(e => e.DateOfBirth.Day);

                sortedEmployees.AddRange(orderedEmployees.Select(employee => new EmployeeBirthdayViewModel
                {
                    DateOfBirth = employee.DateOfBirth.ToString("dd-MMM-yyyy"),
                    FullName = employee.FullName,
                    Month = employee.DateOfBirth.Month,
                    NickName = employee.NickName
                }));
            }

            return sortedEmployees;
        }

        private IEnumerable<Employee> GetEmployees()
        {
            var getAllEmployeeCriteria = new GetEmployeeCriteria
            {
                Query = string.Empty,
                TeamId = null,
                JobTitleId = null,
                EnableStatus = null,
                DeleteStatus = null,
                PageNumber = 1,
                PageSize = int.MaxValue,
                TotalRow = 2000
            };

            return _employeeService.Query(getAllEmployeeCriteria);
        }

        private Employee GetEmployeeById(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId
            };

            return _employeeService.GetEmployeeById(criteria);
        }

        private static DefaultScheduleViewModel GetDefaultScheduleFromEmployee(Employee employee)
        {
            var defaultSchedule = new DefaultScheduleViewModel
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

        private static IList<Employee> SortEmployee(string sort, IList<Employee> employees)
        {
            IList<Employee> employeesSorted;

            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);
                employeesSorted = Utilities.MultipleSort(employees, sorts);
            }
            else
            {
                employeesSorted = employees;
            }

            return employeesSorted;
        }
    }
}