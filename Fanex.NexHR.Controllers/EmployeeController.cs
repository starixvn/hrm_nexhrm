namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Model;
    using Criteria.Employee;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.History;
    using Fanex.NexHR.Criteria.JobGrade;
    using Fanex.NexHR.Criteria.ManagementGrade;
    using Fanex.NexHR.Criteria.TrainingAndDevelopment;
    using Fanex.NexHR.Resources;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using ViewModels;
    using ViewModels.Employee;

    public class EmployeeController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IJobTitleService jobTitleService;
        private readonly IShiftService shiftService;
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;
        private readonly IMailEmployeeService mailEmployeeService;
        private readonly ITrainingAndDevelopmentService trainingAndDevelopmentService;
        private readonly IHistoryService historyService;
        private readonly IWebHostEnvironment environment;
        private readonly IAuthService authService;
        private readonly AppSetting appSetting;

#pragma warning disable S107 // Methods should not have too many parameters

        public EmployeeController(ISessionService sessionService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IJobTitleService jobTitleService,
            IShiftService shiftService,
            IBaseService baseService,
            IMailEmployeeService mailEmployeeService,
            ITrainingAndDevelopmentService trainingAndDevelopmentService,
            IHistoryService historyService,
            IAuthService authService,
            IWebHostEnvironment environment,
            AppSetting appSetting) : base(sessionService)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.jobTitleService = jobTitleService;
            this.shiftService = shiftService;
            this.baseService = baseService;
            this.mailEmployeeService = mailEmployeeService;
            this.environment = environment;
            this.trainingAndDevelopmentService = trainingAndDevelopmentService;
            this.historyService = historyService;
            this.authService = authService;
            this.appSetting = appSetting;
        }

        public async Task<IActionResult> Index()
        {
            var model = new StaffInfoViewModel { Teams = await teamService.GetTeam() };

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> UserManagement(int jobTitleId = 0)
        {
            var model = new UserManagementViewModel
            {
                JobTitleId = jobTitleId,
                Teams = await teamService.GetTeam(),
                IsSpecial = false,
                JobTitles = await jobTitleService.GetAll()
            };

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> SpecialUserManagement(int jobTitleId = 0)
        {
            var model = new UserManagementViewModel
            {
                JobTitleId = jobTitleId,
                Teams = await teamService.GetTeam(),
                IsSpecial = true,
                JobTitles = await jobTitleService.GetAll(),
            };

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<JsonResult> GetEmployees(
            string query,
            int? teamId = null,
            bool? isDisable = null,
            bool? isDelete = null,
            int? jobTitleId = null,
            string sort = null,
            bool? isSpecial = null)
        {
            var listEmployee = new EmployeeListViewModel
            {
                IsShowResignedDate = isDisable.HasValue && isDisable.Value || isDelete.HasValue && isDelete.Value
            };

            var criteria = new GetEmployeeCriteria
            {
                Query = query ?? string.Empty,
                TeamId = teamId == -1 ? null : teamId,
                JobTitleId = jobTitleId,
                IsDisable = isDisable,
                IsDelete = isDelete,
                PageNumber = 1,
                PageSize = int.MaxValue,
                TotalRow = 2000,
                IsSpecial = isSpecial
            };

            listEmployee.Employees = await employeeService.Query(criteria);
            listEmployee.Employees = SortEmployee(sort, listEmployee.Employees);

            return Json(listEmployee);
        }

        public async Task<JsonResult> GetStaffInfo(
            string query,
            int? teamId = null,
            string sort = null)
        {
            var criteria = new GetStaffInfoCriteria
            {
                Query = query ?? string.Empty,
                TeamId = teamId == -1 ? null : teamId,
                IsSpecial = false
            };

            var staffs = await baseService.GetDataAsync<StaffInfo>(criteria);
            staffs = SortStaffInfo(sort, staffs.ToList());

            return Json(staffs);
        }

        public async Task<JsonResult> GetEmployeeSuggestion(
            string query,
            int? teamId = null,
            bool? isSpecial = null)
        {
            var criteria = new GetStaffInfoCriteria
            {
                Query = query ?? string.Empty,
                TeamId = teamId == -1 ? null : teamId,
                IsSpecial = isSpecial
            };

            var staffs = await baseService.GetDataAsync<StaffInfo>(criteria);

            return Json(staffs);
        }

        public async Task<JsonResult> GetGeoMapEmployeeSuggestion(string query)
        {
            var criteria = new GetGeoMapInfoCriteria
            {
                Query = query ?? string.Empty
            };

            var staffs = await baseService.GetDataAsync<GeoMapInfo>(criteria);

            return Json(staffs);
        }

        public async Task<IActionResult> Detail(int employeeId)
        {
            Employee employee = await GetEmployeeById(employeeId);
            employee.TrainingAndDevelopments = await trainingAndDevelopmentService.GetByEmpId(new GetTrainingAndDevelopmentByEmpIdCriteria
            {
                EmpId = employeeId,
                CurrentUserId = CurrentUser.Id
            }, CurrentUser);
            employee.TrainingAndDevelopmentTypes = await trainingAndDevelopmentService.GetTrainingAndDevelopmentType();
            employee.HistoryWork = await historyService.GetByEmpId(new GetHistoryByEmpIdCriteria
            {
                EmpId = employeeId
            });

            return View(employee);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create(
            DateTime? DOB,
            DateTime? joinDate,
            byte? gradeId,
            string fullName,
            string phone,
            string educationBackground)
        {
            ViewBag.DomainName = appSetting.DomainName;
            var model = new CreateEmployeeViewModel
            {
                MonShift = 1,
                TueShift = 1,
                WedShift = 1,
                ThuShift = 1,
                FriShift = 1,
                Teams = await teamService.GetAll(),
                Shifts = await shiftService.GetAll(),
                FullName = fullName,
                Phone = phone,
                DateOfBirth = DOB,
                JoinDate = joinDate,
                EducationBackground = educationBackground,
                GradeId = gradeId
            };

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                model.DomainUsername = appSetting.DomainName + model.DomainUsername;
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

                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.IsSuccessful)
                {
                    return RedirectToAction("Edit", new
                    {
                        id = criteria.EmployeeId,
                        phone = model.Phone,
                        DOB = model.DateOfBirth,
                        joinDate = model.JoinDate,
                        educationBackground = model.EducationBackground,
                        gradeId = model.GradeId
                    });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }

                model.Teams = await teamService.GetAll();
                model.Shifts = await shiftService.GetAll();
            }

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(
            int id,
            DateTime? DOB,
            DateTime? joinDate,
            byte? gradeId,
            string phone,
            string educationBackground)
        {
            Employee employee = await GetEmployeeById(id);
            employee.Phone = phone ?? employee.Phone;
            employee.DateOfBirth = DOB ?? employee.DateOfBirth;
            employee.JoinedDate = joinDate ?? employee.JoinedDate;
            employee.GroupDate = joinDate ?? employee.GroupDate;
            employee.EducationBackground = educationBackground ?? employee.EducationBackground;

            EditEmployeeViewModel model = new EditEmployeeViewModel(employee)
            {
                Teams = await teamService.GetAll(),
                JobTitles = await jobTitleService.GetAll(),
                LastUrl = employee.IsSpecialUser ? "Employee/SpecialUserManagement" : "Employee/Usermanagement"
            };

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditEmployeeViewModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return View(model);
            }

            UpdateEmployeeCriteria criteria = FillDataUpdateEmployeeCriteria(model);
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            if (result.IsSuccessful)
            {
                ViewBag.Updated = true;
                if (CurrentUser.Id == model.Id)
                {
                    var currentUser = await authService.GetEmployeeByDomain(model.DomainUsername);
                    HttpContext.Session.Set("CurrentUser", currentUser);
                }

                Task.Factory.StartNew(async () =>
                {
                    var employee = await GetEmployeeById(criteria.EmployeeId);
                    mailEmployeeService.SendMailUpdateSuccess(employee);
                });
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
            }

            model.Teams = await teamService.GetAll();
            model.JobTitles = await jobTitleService.GetAll();

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Delete(int employeeId, DateTime resignedDate)
        {
            var criteria = new DeleteEmployeeCriteria
            {
                EmployeeId = employeeId,
                ResignedDate = resignedDate,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> DefaultSchedule(int id)
        {
            Employee employee = await GetEmployeeById(id);
            DefaultScheduleViewModel model = GetDefaultScheduleFromEmployee(employee);
            model.Shifts = await shiftService.GetAll();

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> DefaultSchedule(DefaultScheduleViewModel model)
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
                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }

                model.Shifts = await shiftService.GetAll();
            }

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> MonthSchedule(int employeeId)
        {
            Employee employee = await GetEmployeeById(employeeId);
            MonthlyScheduleViewModel model = new MonthlyScheduleViewModel();
            if (employee != null)
            {
                model.EmployeeId = employee.Id;
                model.EmployeeNickName = employee.NickName;
            }

            model.Shifts = await shiftService.GetAll();
            return View(model);
        }

        [AllowAnonymous]
        public FileResult Avatar(string id)
        {
            string fileName = "/common_files/avatars/avatar.jpg";
            string userId = id;

            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = CurrentUser.Code + ".jpg";
            }
            else
            {
                userId += ".jpg";
            }

            var paths = new[] { environment.WebRootPath, "common_files", "avatars/", userId };
            var avaPath = Path.Combine(paths);

            if (System.IO.File.Exists(avaPath))
            {
                fileName = "/common_files/avatars/" + userId;
            }

            return File(fileName, "image/jpeg");
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<JsonResult> UploadAvatar(string code, IFormFile file)
        {
            if (file != null)
            {
                string FileName = code + ".jpg";
                var paths = new[] { environment.WebRootPath, "common_files", "avatars", FileName };
                var fullPath = Path.Combine(paths);

                var stream = new FileStream(fullPath, FileMode.Create);
                await CopyFiles(file, stream);
                stream.Close();
                return Json(true);
            }
            return Json(null);
        }

        private static async Task CopyFiles(IFormFile file, FileStream stream)
        {
            await file.CopyToAsync(stream);
        }

        public async Task<IActionResult> Birthday(int month = 0)
        {
            IEnumerable<Employee> employees = await GetEmployees();
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

        public async Task<IActionResult> MyProfile()
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = CurrentUser.Id
            };
            Employee employee = await employeeService.GetEmployeeById(criteria);
            employee.TrainingAndDevelopments = await trainingAndDevelopmentService.GetByEmpId(new GetTrainingAndDevelopmentByEmpIdCriteria
            {
                EmpId = CurrentUser.Id,
                CurrentUserId = CurrentUser.Id
            }, CurrentUser);
            employee.TrainingAndDevelopmentTypes = await trainingAndDevelopmentService.GetTrainingAndDevelopmentType();
            return View(employee);
        }

        public async Task<IActionResult> ViewHistoryJobGrade(int id)
        {
            var criteria = new GetEmployeeJobGradeCriteria
            {
                EmployeeId = id,
                IsOnlyCurrentUsed = false
            };
            var employeeJobGrades = await baseService.GetDataAsync<EmployeeJobGrade>(criteria);
            return PartialView("ViewHistoryJobGrade", employeeJobGrades);
        }

        public async Task<IActionResult> ViewHistoryManagementGrade(int id)
        {
            var criteria = new GetEmployeeManagementGradeCriteria
            {
                EmployeeId = id,
                IsOnlyCurrentUsed = false
            };
            var employeeManagementGrades = await baseService.GetDataAsync<EmployeeManagementGrade>(criteria);
            return PartialView("ViewHistoryManagementGrade", employeeManagementGrades);
        }


        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Transfer()
        {
            var luminetGroups = await employeeService.GetLuminetGroups();
            var luminetShifts = await employeeService.GetLuminetShifts();
            var model = new TransferEmployeeViewModel
            {
                LumiGroups = luminetGroups.ToList(),
                LumiShifts = luminetShifts.ToList(),
                LastWorkingDate = DateTimeOffset.Now.DateTime,
                AffectedDate = DateTimeOffset.Now.DateTime
            };

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Transfer(TransferEmployeeViewModel model)
        {
            if (model == null)
            {
                return View((TransferEmployeeViewModel)null);
            }
            var luminetGroups = await employeeService.GetLuminetGroups();
            var luminetShifts = await employeeService.GetLuminetShifts();
            model.LumiGroups = luminetGroups.ToList();
            model.LumiShifts = luminetShifts.ToList();
            var employee = await employeeService.GetEmployeeById(new GetEmployeeByIdCriteria { EmployeeId = model.EmployeeId });
            if (model.EmployeeId <= 0)
            {
                ModelState.AddModelError(string.Empty, Messages.NexEmployeeRequireError);
                return View(model);
            }
            if (!employee.Enabled)
            {
                ModelState.AddModelError(string.Empty, Messages.EmployeeTransferExitsts);
                return View(model);
            }
            var criteria = new TransferEmployeeCriteria
            {
                EmployeeId = model.EmployeeId,
                LastWorkingDate = model.LastWorkingDate,
                AffectedDate = model.AffectedDate,
                ToGroupId = model.GroupId,
                ShiftId = model.ShiftId,
                IsTransferAnnualLeaveBalance = model.IsTransferAnnualLeaveBalance ? 1 : 0,
                IsTransferLateInEarlyOut = model.IsTransferLateInEarlyOut ? 1 : 0,
                ModifiedBy = CurrentUser.Id,
                IsTransferNexBalance = model.IsTransferLateInEarlyOut ? 1 : 0
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
            if (result.Code == 0 || result.Code == -1)
            {
                ViewBag.Requested = true;
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
            }
            return View(model);
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
                IsSpecialUser = model.IsSpecialUser,
                FullName = model.FullName,
                Gender = model.Gender,
                Geomaplatitude = model.GeoMapLatitude,
                Geomaplongtitude = model.GeoMapLongtitude,
                EmployeeId = model.Id,
                IdCardNo = model.CardNo,
                IssuedDate = model.IssuedDate,
                IssuedPlace = model.IssuedPlace,
                JobTitleId = model.JobTitleId,
                JoinedDate = model.JoinedDate,
                TeamId = model.TeamId,
                Marriage = model.Marriage,
                ModifiedBy = CurrentUser.Id,
                NickName = model.NickName,
                PassportExp = model.PassportExp,
                PassportNo = model.PassportNo,
                SocialInsuranceNo = model.SocialInsuranceNo,
                Phone = model.Phone,
                ResignedDate = model.ResignedDate,
                Roles = model.Roles,
                Skype = model.Skype,
                SubGroups = model.SubGroups,
                Yahoo = model.Yahoo,
                EmployeeCard = model.EmployeeCard,
                PlaceOfBirth = model.PlaceOfBirth,
                ContactName = model.ContactName,
                UserRelationship = model.UserRelationship,
                ContactPhone = model.ContactPhone,
                ContactAddress = model.ContactAddress,
                GroupDate = model.GroupDate,
                EducationBackground = model.EducationBackground
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

        private async Task<IEnumerable<Employee>> GetEmployees()
        {
            var getAllEmployeeCriteria = new GetEmployeeCriteria
            {
                Query = string.Empty,
                TeamId = null,
                JobTitleId = null,
                IsDisable = null,
                IsDelete = null,
                IsSpecial = false,
                PageNumber = 1,
                PageSize = int.MaxValue,
                TotalRow = 2000
            };

            return await employeeService.Query(getAllEmployeeCriteria);
        }

        private async Task<Employee> GetEmployeeById(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId
            };

            return await employeeService.GetEmployeeById(criteria);
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

        private static IList<StaffInfo> SortStaffInfo(string sort, IList<StaffInfo> staffInfos)
        {
            IList<StaffInfo> employeesSorted;

            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);
                employeesSorted = Utilities.MultipleSort(staffInfos, sorts);
            }
            else
            {
                employeesSorted = staffInfos;
            }

            return employeesSorted;
        }
    }
}