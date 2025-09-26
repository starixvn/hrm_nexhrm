namespace Fanex.NexHR.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Attributes;
using Core;
using Core.Enums;
using Core.Helpers;
using Core.Model;
using Criteria.Employee;
using Criteria.History;
using Criteria.TrainingAndDevelopment;
using Fanex.NexHR.Core.Services;
using Fanex.NexHR.Criteria.JobGrade;
using Fanex.NexHR.Criteria.ManagementGrade;
using GO.UserControl.Client.Core.Models.User;
using HRM.Criteria.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using Resources;
using Services.Interfaces;
using ViewModels;
using ViewModels.Employee;
using GOModels = GO.UserControl.Client.Core.Models;
using GOServices = GO.UserControl.Client.Core.Services;

public class EmployeeController : BaseController
{
    private const string ChangePasswordViewName = "ChangePassword";

    private readonly ITeamService teamService;
    private readonly IDepartmentService departmentService;
    private readonly IJobTitleService jobTitleService;
    private readonly IShiftService shiftService;
    private readonly IEmployeeService employeeService;
    private readonly IBaseService baseService;
    private readonly IMailEmployeeService mailEmployeeService;
    private readonly ITrainingAndDevelopmentService trainingAndDevelopmentService;
    private readonly IHistoryService historyService;
    private readonly IAuthService authService;
    private readonly AppSetting appSetting;
    private readonly ILookupDataService lookupDataService;
    private readonly IMinIOService minIOService;
    private readonly GOServices.ICryptoService cryptoService;
    private readonly GOServices.IUserService userService;

#pragma warning disable S107 // Methods should not have too many parameters

    public EmployeeController(
        AppSetting appSetting,
        ISessionService sessionService,
        IEmployeeService employeeService,
        ITeamService teamService,
        IDepartmentService departmentService,
        IJobTitleService jobTitleService,
        IShiftService shiftService,
        IBaseService baseService,
        IMailEmployeeService mailEmployeeService,
        ITrainingAndDevelopmentService trainingAndDevelopmentService,
        IHistoryService historyService,
        IAuthService authService,
        IMinIOService minIOService,
        ILookupDataService lookupDataService,
        GOServices.ICryptoService cryptoService,
        GOServices.IUserService userService) : base(sessionService)
#pragma warning restore S107 // Methods should not have too many parameters
    {
        this.employeeService = employeeService;
        this.teamService = teamService;
        this.departmentService = departmentService;
        this.jobTitleService = jobTitleService;
        this.shiftService = shiftService;
        this.baseService = baseService;
        this.mailEmployeeService = mailEmployeeService;
        this.trainingAndDevelopmentService = trainingAndDevelopmentService;
        this.historyService = historyService;
        this.authService = authService;
        this.appSetting = appSetting;
        this.lookupDataService = lookupDataService;
        this.minIOService = minIOService;
        this.cryptoService = cryptoService;
        this.userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new StaffInfoViewModel
        {
            Teams = await teamService.GetTeam(),
            Departments = await departmentService.GetAll()
        };

        return View(model);
    }

    [HrmAuthorize(UserRoles = "HR")]
    public async Task<IActionResult> UserManagement(int jobTitleId = 0)
    {
        var model = new UserManagementViewModel
        {
            JobTitleId = jobTitleId,
            Teams = await teamService.GetTeam(),
            Departments = await departmentService.GetAll(),
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
            Teams = await teamService.GetAll(),
            Departments = await departmentService.GetAll(null),
            IsSpecial = true,
            JobTitles = await jobTitleService.GetAll(),
        };

        return View(model);
    }

    [HrmAuthorize(UserRoles = "HR")]
    public async Task<JsonResult> GetEmployees(EmployeeListFilterViewModel filter)
    {
        var listEmployee = new EmployeeListViewModel
        {
            IsShowResignedDate = filter.IsDisable.HasValue && filter.IsDisable.Value || filter.IsDelete.HasValue && filter.IsDelete.Value
        };

        var criteria = new GetEmployeeCriteria
        {
            Query = filter.Query ?? string.Empty,
            TeamId = filter.TeamId == -1 ? null : filter.TeamId,
            DepartmentId = filter.DepartmentId == -1 ? null : filter.DepartmentId,
            JobTitleId = filter.JobTitleId,
            IsDisable = filter.IsDisable,
            IsDelete = filter.IsDelete,
            PageNumber = 1,
            PageSize = int.MaxValue,
            TotalRow = 2000,
            IsSpecial = filter.IsSpecial,
        };

        listEmployee.Employees = await employeeService.Query(criteria);
        listEmployee.Employees = SortEmployee(filter.Sort, listEmployee.Employees);

        return Json(listEmployee);
    }

    public async Task<JsonResult> GetStaffInfo(
        string query,
        int? teamId = null,
        int? departmentId = null,
        string sort = null)
    {
        var criteria = new GetStaffInfoCriteria
        {
            Query = query ?? string.Empty,
            TeamId = teamId == -1 ? null : teamId,
            DepartmentId = departmentId == -1 ? null : departmentId,
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
        var employee = await GetEmployeeById(employeeId);
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
        ViewBag.SalaryGroups = lookupDataService.GetSalaryGroups();

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
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Code);

            var isExistCriteria = new CheckExistEmployeeCriteria
            {
                Code = model.Code,
                Nickname = model.NickName,
            };

            var commandResult = await employeeService.CheckExistAsync(isExistCriteria);
            if (!commandResult.IsSuccessful)
            {
                ModelState.AddModelError(string.Empty, commandResult.Message);
            }
            var createUserResponse = await CreateUserAsync(model);

            if (!createUserResponse.IsSuccessful)
            {
                ModelState.AddModelError(string.Empty, createUserResponse.Message);
            }

            var criteria = new InsertEmployeeCriteria
            {
                Code = model.Code,
                Password = hashedPassword,
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
                WedShift = model.WedShift,
                UserId = createUserResponse.ObjectId
            };

            var result = await baseService.ExecuteNonQueryAsync(criteria);
            if (result.IsSuccessful)
            {
                return RedirectToAction("Edit", new
                {
                    id = criteria.EmpId,
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

    private async Task<GOModels.CommandResult> CreateUserAsync(CreateEmployeeViewModel model)
    {
        await authService.AuthorizeGoSystemAsync();
        var token = cryptoService.GenerateRijndaelKeyIV();
        var keyIv = token.Split(':');
        var rijndaelKey = keyIv[0];
        var rijndaelIv = keyIv[1];

        var param = new CreateUserParam
        {
            Code = model.Code,
            Nickname = model.NickName,
            FullName = model.FullName,
            BCryptPassword = cryptoService.Encrypt(model.Code, rijndaelKey, rijndaelIv),
            Token = token
        };
        var commandResult = await userService.CreateAsync(param);

        return commandResult;
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
        var employee = await GetEmployeeById(id);
        employee.Phone = phone ?? employee.Phone;
        employee.DateOfBirth = DOB ?? employee.DateOfBirth;
        employee.JoinedDate = joinDate ?? employee.JoinedDate;
        employee.GroupDate = joinDate ?? employee.GroupDate;
        employee.EducationBackground = educationBackground ?? employee.EducationBackground;

        var model = new EditEmployeeViewModel(employee)
        {
            Teams = await teamService.GetAll(),
            JobTitles = await jobTitleService.GetAll(),
            LastUrl = employee.IsSpecialUser ? "Employee/SpecialUserManagement" : "Employee/Usermanagement"
        };

        ViewBag.SalaryGroups = lookupDataService.GetSalaryGroups();
        ViewBag.PayrollCompanies = lookupDataService.GetNexCompanies();
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

        var criteria = FillDataUpdateEmployeeCriteria(model);

        var employee = await GetEmployeeById(criteria.EmployeeId);
        var updatedGoResponse = await UpdateUserAsync(new UpdateUserInfoParam
        {
            RefId = model.Id,
            UserId = employee.Id,
            Code = employee.Code,
            Nickname = employee.NickName,
            FullName = employee.FullName,
            Email = employee.Email,
            IsActive = employee.Enabled
        });

        if (!updatedGoResponse.IsSuccessful)
        {
            ModelState.AddModelError(string.Empty, updatedGoResponse.Message);
        }
        var result = await baseService.ExecuteNonQueryAsync(criteria);

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
                mailEmployeeService.SendMailUpdateSuccess(employee);
            });
        }
        else
        {
            ModelState.AddModelError(string.Empty, result.Message);
        }

        model.Teams = await teamService.GetAll();
        model.JobTitles = await jobTitleService.GetAll();
        ViewBag.SalaryGroups = lookupDataService.GetSalaryGroups();
        ViewBag.PayrollCompanies = lookupDataService.GetNexCompanies();

        return View(model);
    }

    private async Task<GOModels.CommandResult> UpdateUserAsync(UpdateUserInfoParam model)
    {
        await authService.AuthorizeGoSystemAsync();

        var commandResult = await userService.UpdateInfoAsync(new UpdateUserInfoParam
        {
            RefId = model.RefId,
            UserId = model.UserId,
            Code = model.Code,
            Nickname = model.Nickname,
            FullName = model.FullName,
            Email = model.Email,
            IsActive = model.IsActive
        });

        return commandResult;
    }

    [HrmAuthorize(UserRoles = "HR")]
    [HttpPost]
    public async Task<IActionResult> Delete(int employeeId, DateTime resignedDate)
    {
        var employee = await GetEmployeeById(employeeId);
        var criteria = new DeleteEmployeeCriteria
        {
            EmployeeId = employeeId,
            ResignedDate = resignedDate,
            ModifiedBy = CurrentUser.Id
        };
        await authService.AuthorizeGoSystemAsync();
        var response = await userService.DeleteAsync(employee.UserId);

        if (!response.IsSuccessful)
        {
            ModelState.AddModelError(string.Empty, response.Message);
        }
        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return Json(result);
    }

    [HrmAuthorize(UserRoles = "HR")]
    public async Task<IActionResult> DefaultSchedule(int id)
    {
        var employee = await GetEmployeeById(id);
        var model = GetDefaultScheduleFromEmployee(employee);
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
            var result = await baseService.ExecuteNonQueryAsync(criteria);
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
        var employee = await GetEmployeeById(employeeId);
        var model = new MonthlyScheduleViewModel();
        if (employee != null)
        {
            model.EmployeeId = employee.Id;
            model.EmployeeNickName = employee.NickName;
        }

        model.Shifts = await shiftService.GetAll();
        return View(model);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Avatar(string id)
    {
        var stream = await FileHelpers.GetAvatarFileContent(id);
        return File(stream, "image/jpeg", $"{id}.jpg");
    }

    [HrmAuthorize(UserRoles = "HR")]
    [HttpPost]
    public async Task<IActionResult> UploadAvatar(string code, IFormFile file)
    {
        if (file is not { Length: > 0 })
        {
            return Json(null);
        }

        var path = $"1/{code}.jpg";
        var uploadStatus = await minIOService.UploadFileAsync(file, StorageInfo.Avatar.Bucket, path);
        if (!uploadStatus)
        {
            return Json(null);
        }

        return Ok();
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
        var employee = await employeeService.GetEmployeeById(criteria);
        employee.TrainingAndDevelopments = await trainingAndDevelopmentService.GetByEmpId(new GetTrainingAndDevelopmentByEmpIdCriteria
        {
            EmpId = CurrentUser.Id,
            CurrentUserId = CurrentUser.Id
        }, CurrentUser);
        employee.TrainingAndDevelopmentTypes = await trainingAndDevelopmentService.GetTrainingAndDevelopmentType();
        ViewBag.SalaryGroups = lookupDataService.GetSalaryGroups();

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
            LastWorkingDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
            AffectedDate = DateTimeOffset.Now.DateTime.ToClientTimeZone()
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
        var result = await baseService.ExecuteNonQueryAsync(criteria);
        if (result.Code is 0 or -1)
        {
            ViewBag.Requested = true;
        }
        else
        {
            ModelState.AddModelError(string.Empty, result.Message);
        }
        return View(model);
    }

    public IActionResult ChangePassword()
    {
        return View(ChangePasswordViewName);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitChangePassword(ChangePasswordViewModel model)
    {
        if (model.ConfirmNewPassword != model.NewPassword)
        {
            ModelState.AddModelError(string.Empty, Messages.PasswordNotMatch);
            return View(ChangePasswordViewName);
        }

        var userLogin = await authService.GetLoginEmployee(HttpContext.User.Identity.Name);
        if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, userLogin.Password))
        {
            ModelState.AddModelError(string.Empty, Messages.IncorrectCurrentPassword);
            return View(ChangePasswordViewName);
        }

        var commandResult = await authService.ChangePassword(CurrentUser.Id, BCrypt.Net.BCrypt.HashPassword(model.NewPassword));
        if (commandResult.IsSuccessful)
        {
            ViewBag.Success = true;
            return View(ChangePasswordViewName);
        }
        ModelState.AddModelError(string.Empty, Messages.FailedToChangePassword);
        return View(ChangePasswordViewName);
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
            AutoApproveRequest = model.AutoApproveRequest,
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
            EducationBackground = model.EducationBackground,
            SalaryGroupId = model.SalaryGroupId,
            PayrollCompanyId = model.PayrollCompanyId
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