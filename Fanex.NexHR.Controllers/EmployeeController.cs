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
    using Fanex.NexHR.Core.Model;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Employee;
    using FPS.Client;
    using Newtonsoft.Json;
    using Simplicode.Imaging;
    using Simplicode.Imaging.Filters;

    public class EmployeeController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IJobTitleService jobTitleService;
        private readonly IShiftService shiftService;
        private readonly IEmployeeService employeeService;
        private readonly ICommandService commandService;

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
            this.employeeService = employeeService ?? new EmployeeService();
            this.teamService = teamService ?? new TeamService();
            this.jobTitleService = jobTitleService ?? new JobTitleService();
            this.shiftService = shiftService ?? new ShiftService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            var model = new StaffInfoViewModel();
            model.Teams = teamService.GetAll();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult UserManagement(int jobTitleId = 0)
        {
            var model = new UserManagementViewModel()
            {
                JobTitleId = jobTitleId,
                Teams = teamService.GetAll(),
                JobTitles = jobTitleService.GetAll()
            };

            return View(model);
        }

        public JsonResult GetEmployees(
            string query,
            int? teamId = null,
            bool? enable = null,
            bool? isDelete = null,
            int? jobTitleId = null,
            string sort = null)
        {
            if (teamId == -1)
            {
                teamId = null;
            }

            var criteria = new GetEmployeeCriteria()
            {
                Query = query,
                TeamId = teamId,
                JobTitleId = jobTitleId,
                Enable = CheckUserEnableCondition(enable),
                IsDelete = CheckDeletedUserCondition(isDelete),
                PageNumber = 1,
                PageSize = int.MaxValue,
                TotalRow = 2000
            };

            var employees = employeeService.Query(criteria);

            employees = SortEmployee(sort, employees);

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int employeeId)
        {
            Employee employee = GetEmployeeById(employeeId);
            return View(employee);
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
                Teams = teamService.GetAll(),
                Shifts = shiftService.GetAll()
            };

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(CreateEmployeeViewModel model)
        {
            if (model != null && ModelState.IsValid)
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

                CommandResult result = commandService.Insert(criteria, hasGetOutputParams: true);
                if (result.IsSuccessful)
                {
                    return Redirect("Edit?id=" + criteria.EmployeeId);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }

                model.Teams = teamService.GetAll();
                model.Shifts = shiftService.GetAll();
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            Employee employee = GetEmployeeById(id);

            EditEmployeeViewModel model = new EditEmployeeViewModel(employee)
            {
                Teams = teamService.GetAll(),
                JobTitles = jobTitleService.GetAll()
            };

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditEmployeeViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                UpdateEmployeeCriteria criteria = FillDataUpdateEmployeeCriteria(model);
                CommandResult result = commandService.Update(criteria);

                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                    if (CurrentUser.Id == model.Id)
                    {
                        var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = model.Id };
                        Employee currentUser = employeeService.GetEmployeeById(getEmployeeCriteria);
                        HttpContext.Session["CurrentUser"] = currentUser;
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }

                model.Teams = teamService.GetAll();
                model.JobTitles = jobTitleService.GetAll();
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int employeeId, DateTime resignedDate)
        {
            var criteria = new DeleteEmployeeCriteria()
            {
                EmployeeId = employeeId,
                ResignedDate = resignedDate
            };
            CommandResult result = commandService.Delete(criteria);

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        public ActionResult DefaultSchedule(int employeeId)
        {
            Employee employee = GetEmployeeById(employeeId);
            DefaultScheduleViewModel model = GetDefaultScheduleFromEmployee(employee);
            model.Shifts = shiftService.GetAll();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult DefaultSchedule(DefaultScheduleViewModel model)
        {
            if (model != null && ModelState.IsValid)
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
                CommandResult result = commandService.Update(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }

                model.Shifts = shiftService.GetAll();
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

            model.Shifts = shiftService.GetAll();
            return View(model);
        }

        [AllowAnonymous]
        public FileResult Avatar(string id)
        {
            string fileName = "/avatars/avatar.jpg";
            if (string.IsNullOrWhiteSpace(id))
            {
                id = CurrentUser.Code + ".jpg";
            }
            else
            {
                id += ".jpg";
            }

            if (System.IO.File.Exists(Server.MapPath("/avatars/" + id)))
            {
                fileName = "/avatars/" + id;
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
                        var resizer = new ResizeFilter(64, 64, ResizeMethod.Crop, AnchorLocation.Middle);

                        processor.AddFilter(resizer);
                        processor.OutputFormat = OutputFormat.Jpeg;
                        processor.JpegCompression = 100;
                        processor.ProcessImage(file.InputStream, stream);

                        var image = Image.FromStream(stream);
                        image.Save(Server.MapPath("/avatars/") + code + ".jpg");

                        return Json(true);
                    }
                }
            }

            return Json(null);
        }

        public ActionResult MySpace()
        {
            var getMySpaceCriteria = new GetMySpaceCriteria()
            {
                EmpId = CurrentUser.Id
            };
            var model = employeeService.GetMySpace(getMySpaceCriteria);

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
                var model = employeeService.GetHRLeaderDashboard(criteria);
                return PartialView(model);
            }

            return Content(string.Empty);
        }

        [HttpPost]
        public ActionResult PostFpsTransaction()
        {
            using (var serviceFPS = new Service())
            {
                var resultMessage = string.Empty;
                var deviceCode = string.Empty;
                serviceFPS.CheckTransaction(CurrentUser.NickName, "login", string.Empty, "SubTest2", out resultMessage, out deviceCode);

                return Content(string.Empty);
            }
        }

        public ActionResult Birthday(int month = 0)
        {
            IEnumerable<Employee> employees = GetEmployees();
            IList<EmployeeBirthdayViewModel> sortedEmployees;
            if (month != 0)
            {
                sortedEmployees = SortEmployeeBirthdayByMonth(employees.Where(e => e.DateOfBirth.Month == month));
            }
            else
            {
                sortedEmployees = SortEmployeeBirthdayByMonth(employees);
            }

            return View(sortedEmployees);
        }

        public ActionResult MyProfile()
        {
            var criteria = new GetEmployeeByIdCriteria()
            {
                EmployeeId = CurrentUser.Id
            };
            Employee employee = employeeService.GetEmployeeById(criteria);
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

            return Json(employeeService.GetMembersByManager(getMemberByManager).Take(10), JsonRequestBehavior.AllowGet);
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
            MySpaceViewModel myspace = employeeService.GetMySpace(criteria);

            return myspace;
        }

        private IList<EmployeeBirthdayViewModel> SortEmployeeBirthdayByMonth(IEnumerable<Employee> employees)
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

        private IEnumerable<Employee> GetEmployees()
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

            return employeeService.Query(getAllEmployeeCriteria);
        }

        private Employee GetEmployeeById(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria()
            {
                EmployeeId = employeeId
            };

            return employeeService.GetEmployeeById(criteria);
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

        private int? CheckUserEnableCondition(bool? enable)
        {
            int? disableUsers;
            if (enable == true)
            {
                disableUsers = 0;
            }
            else if (enable == false)
            {
                disableUsers = 1;
            }
            else
            {
                disableUsers = null;
            }

            return disableUsers;
        }

        private int? CheckDeletedUserCondition(bool? isDelete)
        {
            int? deleteUsers;
            if (isDelete == true)
            {
                deleteUsers = 1;
            }
            else if (isDelete == false)
            {
                deleteUsers = 0;
            }
            else
            {
                deleteUsers = null;
            }

            return deleteUsers;
        }

        private IList<Employee> SortEmployee(string sort, IList<Employee> employees)
        {
            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);

                employees = Utilities.MultipleSort(employees, sorts);
            }

            return employees;
        }
    }
}