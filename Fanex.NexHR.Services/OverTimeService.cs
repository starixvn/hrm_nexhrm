namespace Fanex.NexHR.Services
{
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels.Overtime;

    public class OvertimeService : IOvertimeService
    {
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment environment;
        private readonly IBaseService baseService;
        private const string FormatDate = "dd MMM, yyyy";
        private readonly string SenderEmail;
        private readonly string BaseUrl;
        private readonly string CompanyMailSign;
        private const int RowStartBody = 2;
        private const int CollumnNickName = 1;
        private const int CollumnDate = 2;
        private const int CollumnDuration = 3;
        private const int CollumnNotes = 4;
        private const string TypeTimeUnique = "yyHHmmssffffff";
        private const string FileXLSX = ".xlsx";
        private const string FileXLS = ".xls";

        public OvertimeService(
            IEmailService emailService,
            IWebHostEnvironment environment,
            IBaseService baseService,
            EmailSettings emailSettings,
            AppSetting appSetting)
        {
            this.emailService = emailService;
            this.environment = environment;
            this.baseService = baseService;
            SenderEmail = emailSettings.Email;
            BaseUrl = appSetting.BaseUrl;
            CompanyMailSign = emailSettings.CompanyMailSign;
        }

        public async Task<IEnumerable<Overtime>> GetOvertime(GetOvertimeCriteria criteria)
        {
            var result = await baseService.FetchMultipleAsync<Overtime, OvertimeRecord>(criteria);
            var overtimes = result.Item1;
            var overtimeRecords = result.Item2;
            foreach (var overtime in overtimes)
            {
                overtime.OvertimeRecords = overtimeRecords.Where(t => t.OvertimeId == overtime.Id).ToList();
            }

            return overtimes;
        }

        public async Task<Overtime> GetOvertimeById(GetOvertimeByIdCriteria criteria)
        {
            var (overtimes, records) = await baseService.FetchMultipleAsync<Overtime, OvertimeRecord>(criteria);
            var overtime = overtimes.FirstOrDefault();
            if (overtime is not null)
            {
                overtime.OvertimeRecords = records;
            }

            return overtime;
        }

        public async Task<ValueTuple<IEnumerable<OvertimeExcelDataViewModel>, IList<int>>> GetEmployeesFromFileExcel(IFormFile file)
        {
            var dataList = new List<OvertimeExcelDataViewModel>();
            var dataError = new List<int>();

            if (file is { Length: > 0 } && IsExcelFile(file))
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                var workSheet = package.Workbook.Worksheets.First();
                var rowCount = workSheet.Dimension.Rows;
                (dataList, dataError) = ReadFileExcel(rowCount, workSheet);
            }

            return (dataList, dataError);
        }

        private static ValueTuple<List<OvertimeExcelDataViewModel>, List<int>> ReadFileExcel(int rowCount, ExcelWorksheet workSheet)
        {
            var dataList = new List<OvertimeExcelDataViewModel>();
            var dataError = new List<int>();
            for (var row = RowStartBody; row <= rowCount; row++)
            {
                if (!IsNullOrWhiteSpace(workSheet, row))
                {
                    if (IsConvertDate(workSheet.Cells[row, CollumnDate].Text) &&
                        IsConvertDuration(workSheet.Cells[row, CollumnDuration].Text))
                    {
                        var data = new OvertimeExcelDataViewModel
                        {
                            NickName = workSheet.Cells[row, CollumnNickName].Text,
                            Date = Convert.ToDateTime(workSheet.Cells[row, CollumnDate].Text, CultureInfo.InvariantCulture),
                            Duration = float.Parse(workSheet.Cells[row, CollumnDuration].Text),
                            Notes = workSheet.Cells[row, CollumnNotes].Text,
                            Unique = DateTimeOffset.Now.ToString(TypeTimeUnique),
                            Row = row
                        };
                        dataList.Add(data);
                    }
                    else
                    {
                        dataError.Add(row);
                    }
                }
            }
            return (dataList, dataError);
        }

        private static bool IsConvertDate(string date) => DateTime.TryParse(date, CultureInfo.InvariantCulture, out _);

        private static bool IsConvertDuration(string Duration) => float.TryParse(Duration, out _);

        private static bool IsNullOrWhiteSpace(ExcelWorksheet workSheet, int row) =>
            string.IsNullOrWhiteSpace(workSheet.Cells[row, CollumnNickName].Text)
            && string.IsNullOrWhiteSpace(workSheet.Cells[row, CollumnDate].Text)
            && string.IsNullOrWhiteSpace(workSheet.Cells[row, CollumnDuration].Text);

        private static bool IsExcelFile(IFormFile file) =>
            (Path.GetExtension(file.FileName).Equals(FileXLSX, StringComparison.OrdinalIgnoreCase) ||
             Path.GetExtension(file.FileName).Equals(FileXLS, StringComparison.OrdinalIgnoreCase));

        public async Task<IEnumerable<Overtime>> GetReportOvertimeHR(ReportOvertimeHRCriteria criteria)
        {
            var result = await baseService.FetchMultipleAsync<Overtime, OvertimeRecord>(criteria);
            var overtimes = result.Item1;
            var overtimeRecords = result.Item2;
            foreach (var overtime in overtimes)
            {
                overtime.OvertimeRecords = overtimeRecords.Where(t => t.OvertimeId == overtime.Id).ToList();
            }

            return overtimes;
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
                                                                    t.NickName,
                                                                    t.FullName,
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
            var overtime = await GetOvertimeById(criteria);
            var emloyees = string.Join(
                                               Constants.BrTag,
                                               overtime.OvertimeRecords.Select(t =>
                                                                string.Format(
                                                                    Constants.ContentEmployeeOvertime,
                                                                    t.NickName,
                                                                    t.FullName,
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
                    EmployeeId = item.EmployeeId
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