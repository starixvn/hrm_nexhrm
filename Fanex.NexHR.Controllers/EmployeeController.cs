namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Core.Model;
    using Criteria.Employee;
    using Fanex.NexHR.Criteria.HistoryWork;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Models.Attributes;
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
        private readonly ICommandService commandService;
        private readonly IHistoryWorkService historyWorkService;
        private readonly IMailEmployeeService mailEmployeeService;
        private readonly IWebHostEnvironment environment;
        private readonly AppSetting appSetting;

        public EmployeeController(ISessionService sessionService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IJobTitleService jobTitleService,
            IShiftService shiftService,
            ICommandService commandService,
            IHistoryWorkService historyWorkService,
            IMailEmployeeService mailEmployeeService,
            IWebHostEnvironment environment,
            AppSetting appSetting) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.jobTitleService = jobTitleService;
            this.shiftService = shiftService;
            this.commandService = commandService;
            this.historyWorkService = historyWorkService;
            this.mailEmployeeService = mailEmployeeService;
            this.environment = environment;
            this.appSetting = appSetting;
        }

        public async Task<ActionResult> Index()
        {
            var model = new StaffInfoViewModel { Teams = await teamService.GetTeam() };

            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> UserManagement(int jobTitleId = 0)
        {
            var model = new UserManagementViewModel
            {
                JobTitleId = jobTitleId,
                Teams = await teamService.GetTeam(),
                JobTitles = await jobTitleService.GetAll()
            };

            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> SpecialUserManagement(int jobTitleId = 0)
        {
            var model = new UserManagementViewModel
            {
                JobTitleId = jobTitleId,
                Teams = await teamService.GetTeam(),
                JobTitles = await jobTitleService.GetAll(),
            };

            return View(model);
        }

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
                IsSpecial = isSpecial ?? false
            };

            listEmployee.Employees = await employeeService.Query(criteria);

            listEmployee.Employees = SortEmployee(sort, listEmployee.Employees);

            return Json(listEmployee);
        }

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> WorkingHistory(int employeeId)
        {
            const int startYear = 2011;
            var history = new WorkingHistoryViewModel { EmpId = employeeId };
            var criteria = new GetEmployeeByIdCriteria { EmployeeId = employeeId };
            history.Employee = await employeeService.GetEmployeeById(criteria);
            history.FromDate = new DateTime(startYear, 1, 1);

            return View(history);
        }

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> HistoryWorkDetail(WorkingHistoryViewModel model)
        {
            IList<HistoryWork> filterHistory = await GetFilterHistoryWork(model);

            return PartialView("_HistoryWorkDetail", filterHistory);
        }

        private async Task<IList<HistoryWork>> GetFilterHistoryWork(WorkingHistoryViewModel model)
        {
            var criteria = new FilterHistoryWorkCriteria
            {
                EmpId = model.EmpId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
            };

            IList<HistoryWork> filterHistoryWork = await historyWorkService.GetFilterHistoryWork(criteria);

            ViewBag.empid = model.EmpId;
            return filterHistoryWork;
        }

        [HttpGet]
        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> InsertHistoryWork(int employeeId)
        {
            var history = new WorkingHistoryViewModel { EmpId = employeeId };
            history.Teams = await teamService.GetTeam();
            history.Employee = await GetEmployeeById(employeeId);
            var lookupDataJobTitleCriteria = new GetJobTitleCriteria { JobTitle = "" };
            history.JobTitles = await jobTitleService.GetJobTitle(lookupDataJobTitleCriteria);

            return View(history);
        }

        [HttpPost]
        [HrmAuthorize(Roles = "HR")]
        public ActionResult InsertHistoryWork(WorkingHistoryViewModel model)
        {
            var result = new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };

            if (model != null && model.EmpId > 0)
            {
                var history = new InsertHistoryWorkCriteria
                {
                    EmpId = model.EmpId,
                    ModifiedBy = CurrentUser.Id,
                    AffectedDate = model.AffectedDate,
                    ModifiedDate = DateTime.Now,
                    JobtitleName = model.JobtitleName,
                    GroupName = model.GroupName
                };

                result = commandService.ExecuteNonQuery(history);
            }

            return Json(result);
        }

        [HttpGet]
        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> EditHistoryWork(int? id)
        {
            var history = new WorkingHistoryViewModel { };

            if (id.HasValue && id.Value > 0)
            {
                var criteria = new EditHistoryWorkCriteria
                {
                    Id = id.Value
                };

                history = await historyWorkService.GetHistoryWorkInfo(criteria);
                history.Teams = await teamService.GetTeam();
                var lookupDataJobTitleCriteria = new GetJobTitleCriteria { JobTitle = "" };
                history.JobTitles = await jobTitleService.GetJobTitle(lookupDataJobTitleCriteria);
                history.Id = id;
            }

            return PartialView(history);
        }

        [HttpPost]
        [HrmAuthorize(Roles = "HR")]
        public ActionResult EditHistoryWork(WorkingHistoryViewModel model)
        {
            var result = new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };

            if (model != null)
            {
                var history = new UpdateHistoryWorkCriteria
                {
                    Id = model.Id,
                    AffectedDate = model.AffectedDate,
                    ModifiedDate = model.ModifiedDate,
                    JobtitleName = model.JobtitleName,
                    GroupName = model.GroupName
                };

                result = commandService.ExecuteNonQuery(history);
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
            }

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        public JsonResult DeleteHistoryWork(int id)
        {
            var criteria = new DeleteHistoryWorkCriteria
            {
                Id = id,
            };

            CommandResult result = commandService.ExecuteNonQuery(criteria);
            return Json(result);
        }

        public async Task<ActionResult> Detail(int employeeId)
        {
            Employee employee = await GetEmployeeById(employeeId);
            return View(employee);
        }

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> Create()
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
                Shifts = await shiftService.GetAll()
            };

            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<ActionResult> Create(CreateEmployeeViewModel model)
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

                CommandResult result = commandService.ExecuteNonQuery(criteria);
                if (result.IsSuccessful)
                {
                    return Redirect("Edit?id=" + criteria.EmployeeId);
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

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> Edit(int id)
        {
            Employee employee = await GetEmployeeById(id);

            EditEmployeeViewModel model = new EditEmployeeViewModel(employee)
            {
                Teams = await teamService.GetAll(),
                JobTitles = await jobTitleService.GetAll(),
                LastUrl = employee.IsSpecialUser ? "Employee/SpecialUserManagement" : "Employee/Usermanagement"
            };

            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditEmployeeViewModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return View(model);
            }

            UpdateEmployeeCriteria criteria = FillDataUpdateEmployeeCriteria(model);
            CommandResult result = commandService.ExecuteNonQuery(criteria);

            if (result.IsSuccessful)
            {
                ViewBag.Updated = true;
                if (CurrentUser.Id == model.Id)
                {
                    var getEmployeeCriteria = new GetEmployeeByIdCriteria { EmployeeId = model.Id };
                    Employee currentUser = await employeeService.GetEmployeeById(getEmployeeCriteria);
                    if (HttpContext.Session != null)
                    {
                        HttpContext.Session.Set("CurrentUser", currentUser);
                    }
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

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int employeeId, DateTime resignedDate)
        {
            var criteria = new DeleteEmployeeCriteria
            {
                EmployeeId = employeeId,
                ResignedDate = resignedDate
            };
            CommandResult result = commandService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> DefaultSchedule(int employeeId)
        {
            Employee employee = await GetEmployeeById(employeeId);
            DefaultScheduleViewModel model = GetDefaultScheduleFromEmployee(employee);
            model.Shifts = await shiftService.GetAll();

            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<ActionResult> DefaultSchedule(DefaultScheduleViewModel model)
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
                CommandResult result = commandService.ExecuteNonQuery(criteria);
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

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> MonthSchedule(int employeeId)
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

            var avaPath = Path.Combine(environment.WebRootPath, "avatars\\" + userId);

            if (System.IO.File.Exists(avaPath))
            {
                fileName = "/avatars/" + userId;
            }

            return File(fileName, "image/jpeg");
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<JsonResult> UploadAvatar(string code, IFormFile file)
        {
            if(file != null)
            {
                string FileName = code + ".jpg";
                var path = Path.Combine(environment.WebRootPath, "avatars\\" + FileName);

                var stream = new FileStream(path, FileMode.Create);
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

        public async Task<ActionResult> Birthday(int month = 0)
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

        public async Task<ActionResult> MyProfile()
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = CurrentUser.Id
            };
            Employee employee = await employeeService.GetEmployeeById(criteria);
            return View(employee);
        }

        [HrmAuthorize(Roles = "AssistantLeader,HR")]
        public async Task<JsonResult> GetMembersByManager(string keyword, int? groupId = null)
        {
            const int Records = 10;
            var role = CurrentUser.IsInRoles(Role.HR) ? Role.HR : Role.AssistantLeader;
            var getMemberByManager = new GetMemberByManagerCriteria
            {
                ManagerId = CurrentUser.Id,
                Role = (short)role,
                Keyword = keyword,
                GroupId = groupId,
                ExcludedManager = false
            };
            var membersByManager = await employeeService.GetMembersByManager(getMemberByManager);

            return Json(membersByManager.Take(Records));
        }

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> Transfer()
        {
            var luminetGroups = await employeeService.GetLuminetGroups();
            var luminetShifts = await employeeService.GetLuminetShifts();
            var model = new TransferEmployeeViewModel
            {
                LumiGroups = luminetGroups.ToList(),
                LumiShifts = luminetShifts.ToList(),
                LastWorkingDate = DateTime.Now,
                AffectedDate = DateTime.Now
            };

            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<ActionResult> Transfer(TransferEmployeeViewModel model)
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
            CommandResult result = commandService.ExecuteNonQuery(criteria);
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
                EmployeeCard = model.EmployeeCard,
                PlaceOfBirth = model.PlaceOfBirth,
                ContactName = model.ContactName,
                UserRelationship = model.UserRelationship,
                ContactPhone = model.ContactPhone,
                ContactAddress = model.ContactAddress,
                GroupDate = model.GroupDate
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
    }
}