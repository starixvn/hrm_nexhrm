namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Criteria.Employee;
    using Criteria.Overtime;
    using Fanex.NexHR.Models.Core;
    using Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Models;
    using Models.Overtime;
    using OfficeOpenXml;
    using Repositories.Interfaces;
    using ViewModels.Overtime;

    public class OvertimeService : IOvertimeService
    {
        private readonly IOvertimeRepository overtimeRepository;
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment environment;
        private readonly IBaseService baseService;
        private const string FormatDate = "dd MMM, yyyy";
        private readonly string SenderEmail;
        private readonly string BaseUrl;
        private readonly string CompanyMailSign;

        public OvertimeService(IOvertimeRepository overtimeRepository,
            IEmailService emailService,
            IWebHostEnvironment environment,
            IBaseService baseService,
            EmailSettings emailSettings,
            AppSetting appSetting)
        {
            this.overtimeRepository = overtimeRepository;
            this.emailService = emailService;
            this.environment = environment;
            this.baseService = baseService;
            SenderEmail = emailSettings.Email;
            BaseUrl = appSetting.BaseUrl;
            CompanyMailSign = emailSettings.CompanyMailSign;
        }

        public async Task<IList<Overtime>> GetOvertime(GetOvertimeCriteria criteria)
        {
            IList<Overtime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = await overtimeRepository.GetOvertime(criteria);
            }

            return overtimes ?? new List<Overtime>();
        }

        public async Task<Overtime> GetOvertimeById(GetOvertimeByIdCriteria criteria)
        {
            Overtime overtime = null;
            if (criteria != null && criteria.IsValid())
            {
                overtime = await overtimeRepository.GetOvertimeById(criteria);
            }

            return overtime;
        }

        public async Task<IList<EmployeeOvertime>> GetEmployeeOvertime(GetEmployeeOvertimeCriteria criteria)
        {
            var overtimes = await baseService.GetDataAsync<EmployeeOvertime>(criteria);
            return overtimes.ToList();
        }

        public async Task<IList<EmployeeOvertime>> GetMyOvertime(GetMyOvertimeCriteria criteria)
        {
            var overtimes = await baseService.GetDataAsync<EmployeeOvertime>(criteria);
            return overtimes.ToList();
        }

        public async Task<IList<OvertimeSummary>> GetOvertimeSummary(OvertimeSummaryCriteria criteria)
        {
            IList<OvertimeSummary> listSummary = new List<OvertimeSummary>();
            if (criteria != null && criteria.IsValid())
            {
                listSummary = await overtimeRepository.GetOvertimeSummary(criteria);
            }

            return listSummary.ToList();
        }

        public async Task<IList<OvertimeConversion>> GetConversions(GetOvertimeConversionsCriteria criteria)
        {
            IList<OvertimeConversion> conversions = null;
            if (criteria != null && criteria.IsValid())
            {
                conversions = await overtimeRepository.GetConversions(criteria);
            }

            return conversions ?? new List<OvertimeConversion>();
        }

        public async Task<NexHour> GetNexHour(GetNexHourCriteria criteria)
        {
            return await baseService.GetDataEntityAsync<NexHour>(criteria);
        }

        public async Task<IEnumerable<StaffInfoNexHour>> GetListStaffInfoNexHourAsync(GetListStaffInfoNexHourCriteria criteria)
        {
            return await baseService.GetDataAsync<StaffInfoNexHour>(criteria);
        }

        public async Task<IEnumerable<OvertimeExcelDataViewModel>> GetEmployeesFromFileExcel(IFormFile file)
        {
            var dataList = new List<OvertimeExcelDataViewModel>();
            if (file is { Length: > 0 } &&
                (Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                 Path.GetExtension(file.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase)))
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                var workSheet = package.Workbook.Worksheets[0];
                var rowCount = workSheet.Dimension.Rows;

                for (var row = 2; row <= rowCount; row++)
                {
                    var data = new OvertimeExcelDataViewModel
                    {
                        NickName = workSheet.Cells[row, 1].Text,
                        Date = Convert.ToDateTime(workSheet.Cells[row, 2].Text, CultureInfo.InvariantCulture),
                        Duration = float.Parse(workSheet.Cells[row, 3].Text),
                        Notes = workSheet.Cells[row, 4].Text,
                        Unique = DateTimeOffset.Now.ToString("HHmmssfffffff")
                    };
                    dataList.Add(data);
                }
            }

            return dataList;
        }

        public async Task<IList<Overtime>> GetReportOvertimeHR(ReportOvertimeHRCriteria criteria)
        {
            IList<Overtime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = await overtimeRepository.GetOvertime(criteria);
            }

            return overtimes ?? new List<Overtime>();
        }

        public async Task<IEnumerable<ReportOvertimeHRDetail>> GetReportOvertimeHRDetail(ReportOvertimeHRDetailCriteria criteria)
        {
            IEnumerable<ReportOvertimeHRDetail> report = null;
            if (criteria != null && criteria.IsValid())
            {
                report = await baseService.GetDataAsync<ReportOvertimeHRDetail>(criteria);
            }

            return report ?? new List<ReportOvertimeHRDetail>();
        }

        public async Task SendMailForTransferNexBalance(string subject, OvertimeConversionCreateViewModel model)
        {
            GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = model.EmpId
            };
            var receiver = await baseService.GetDataEntityAsync<Employee>(getEmployeeCriteria);

            var templatePath = Path.Combine(environment.ContentRootPath, Constants.EmailTemplatePathForOvertimeConversion);
            var bodyTemplate = Utilities.ReadTextFile(templatePath);
            var body = string.Format(
                    bodyTemplate,
                    receiver.NickName,
                    model.DeductDate.ToDateMonthYearFormatedDate(),
                    model.Deduct,
                    model.Description,
                    BaseUrl,
                    CompanyMailSign);

            emailService.SendEmail(
                    SenderEmail,
                    receiver.Email,
                    string.Empty,
                    subject,
                    body);
        }

        public async Task SendMailForNEXRequest(string subject, CreateOvertimeViewModel model)
        {
            if (model != null)
            {
                var managers = await baseService.GetDataAsync<EmployeeNotificationInfo>(new GetManagementsCriteria());
                var templatePath = Path.Combine(environment.ContentRootPath, Constants.EmailTemplatePathForOvertimeRequest);
                var bodyTemplate = Utilities.ReadTextFile(templatePath);
                var emloyees = string.Join(
                                               Constants.BrTag,
                                               model.TimeSheets.Select(t =>
                                                                string.Format(
                                                                    Constants.ContentEmployeeOvertime,
                                                                    t.Employee.NickName,
                                                                    t.Employee.FullName,
                                                                    t.Date.ToString(FormatDate),
                                                                    t.Hour)));
                if (managers.Any())
                {
                    var body = string.Format(
                        bodyTemplate,
                        model.CreatedByName,
                        model.Description,
                        emloyees,
                        BaseUrl,
                        CompanyMailSign);

                    emailService.SendEmail(
                        SenderEmail,
                        string.Join(";", managers.Select(t => t.Email)),
                        string.Empty,
                        subject,
                        body);
                }
            }
        }

        public async Task SendMailForApprovalNotification(string subject, int overtimeId, int createdBy, int approvedBy)
        {
            GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = approvedBy
            };
            var manager = await baseService.GetDataEntityAsync<Employee>(getEmployeeCriteria);
            var templatePath = Path.Combine(environment.ContentRootPath, Constants.EmailTemplatePathForOvertimeApprovalNotification);
            var bodyTemplate = Utilities.ReadTextFile(templatePath);

            var criteria = new GetOvertimeByIdCriteria
            {
                Id = overtimeId
            };
            var overtime = await overtimeRepository.GetOvertimeById(criteria);
            var emloyees = string.Join(
                                               Constants.BrTag,
                                               overtime.OvertimeRecords.Select(t =>
                                                                string.Format(
                                                                    Constants.ContentEmployeeOvertime,
                                                                    t.Employee.NickName,
                                                                    t.Employee.FullName,
                                                                    t.Date.ToString(FormatDate),
                                                                    t.Hour)));
            _ = SendMailForCreatedRequest(subject, createdBy, manager.NickName, bodyTemplate, emloyees, overtime.Description);
            _ = SendMailForHR(subject, createdBy, manager.NickName, bodyTemplate, emloyees, overtime.Description);
        }

        private async Task SendMailForCreatedRequest(string subject, int createdBy, string managerNickName, string bodyTemplate, string employees, string project)
        {
            GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = createdBy
            };
            var receiver = await baseService.GetDataEntityAsync<Employee>(getEmployeeCriteria);
            var body = string.Format(
                    bodyTemplate,
                    receiver.NickName,
                    "your NEX request",
                    managerNickName,
                    project,
                    employees,
                    BaseUrl,
                    CompanyMailSign);

            emailService.SendEmail(
                    SenderEmail,
                    receiver.Email,
                    string.Empty,
                    subject,
                    body);
        }

        private async Task SendMailForHR(string subject, int createdBy, string managerNickName, string bodyTemplate, string employees, string project)
        {
            IEnumerable<Employee> HRs = await baseService.GetDataAsync<Employee>(new GetHRCriteria());
            var HRLst = HRs.ToList();
            GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = createdBy
            };
            var creator = await baseService.GetDataEntityAsync<Employee>(getEmployeeCriteria);
            var body = string.Format(
                    bodyTemplate,
                    "all",
                    string.Format("{0}'s NEX request", creator.NickName),
                    managerNickName,
                    project,
                    employees,
                    BaseUrl,
                    CompanyMailSign);

            emailService.SendEmail(
                    SenderEmail,
                    string.Join(";", HRLst.Select(t => t.Email)),
                    string.Empty,
                    subject,
                    body);
        }

        public async Task SendMailForNotification(string subject, OvertimeVerifyViewModel model)
        {
            var templatePath = Path.Combine(environment.ContentRootPath, Constants.EmailTemplatePathForOvertimeNotification);
            var bodyTemplate = Utilities.ReadTextFile(templatePath);

            foreach (var item in model.TimeSheets)
            {
                var employee = await baseService.GetDataEntityAsync<Employee>(new GetEmployeeByIdCriteria
                {
                    EmployeeId = item.Employee.Id
                });

                var body = string.Format(
                bodyTemplate,
                employee.NickName,
                item.Date.ToDateMonthYearFormatedDate(),
                item.Hour,
                item.NexHour,
                model.Description,
                BaseUrl,
                CompanyMailSign);

                emailService.SendEmail(
                    SenderEmail,
                    employee.Email,
                    string.Empty,
                    subject,
                    body);
            }
        }
    }
}