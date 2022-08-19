namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using Core;
    using Criteria.Employee;
    using Criteria.Overtime;
    using Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Models;
    using Models.Overtime;
    using Repositories.Interfaces;
    using ViewModels.Overtime;

    public class OvertimeService : IOvertimeService
    {
        private readonly IOvertimeRepository overtimeRepository;
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment environment;
        private readonly IBaseService baseService;
        private const string FormatDate = "dd MMM, yyyy";
        private readonly string SenderEmail = ConfigurationManager.AppSettings["Email"];
        private readonly string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        private readonly string CompanyMailSign = ConfigurationManager.AppSettings["CompanyMailSign"];

        public OvertimeService(IOvertimeRepository overtimeRepository, IEmailService emailService, IWebHostEnvironment environment, IBaseService baseService)
        {
            this.overtimeRepository = overtimeRepository;
            this.emailService = emailService;
            this.environment = environment;
            this.baseService = baseService;
        }

        public IList<Overtime> GetOvertime(GetOvertimeCriteria criteria)
        {
            IList<Overtime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = overtimeRepository.GetOvertime(criteria);
            }

            return overtimes ?? new List<Overtime>();
        }

        public Overtime GetOvertimeById(GetOvertimeByIdCriteria criteria)
        {
            Overtime overtime = null;
            if (criteria != null && criteria.IsValid())
            {
                overtime = overtimeRepository.GetOvertimeById(criteria);
            }

            return overtime;
        }

        public IList<EmployeeOvertime> GetEmployeeOvertime(GetEmployeeOvertimeCriteria criteria)
        {
            IList<EmployeeOvertime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = overtimeRepository.GetEmployeeOvertime(criteria);
            }

            return overtimes ?? new List<EmployeeOvertime>();
        }

        public IList<EmployeeOvertime> GetMyOvertime(GetMyOvertimeCriteria criteria)
        {
            IList<EmployeeOvertime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = overtimeRepository.GetEmployeeOvertime(criteria);
            }

            return overtimes ?? new List<EmployeeOvertime>();
        }

        public List<OvertimeSummary> GetOvertimeSummary(OvertimeSummaryCriteria criteria)
        {
            List<OvertimeSummary> listSummary = new List<OvertimeSummary>();
            if (criteria != null && criteria.IsValid())
            {
                listSummary = overtimeRepository.GetOvertimeSummary(criteria).ToList();
            }

            return listSummary;
        }

        public IList<OvertimeConversion> GetConversions(GetOvertimeConversionsCriteria criteria)
        {
            IList<OvertimeConversion> conversions = null;
            if (criteria != null && criteria.IsValid())
            {
                conversions = overtimeRepository.GetConversions(criteria);
            }

            return conversions ?? new List<OvertimeConversion>();
        }

        public NexHour GetNexHour(GetNexHourCriteria criteria)
        {
            return baseService.GetDataEntity<NexHour>(criteria);
        }

        public IList<Overtime> GetReportOvertimeHR(ReportOvertimeHRCriteria criteria)
        {
            IList<Overtime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = overtimeRepository.GetOvertime(criteria);
            }

            return overtimes ?? new List<Overtime>();
        }

        public IList<ReportOvertimeHRDetail> GetReportOvertimeHRDetail(ReportOvertimeHRDetailCriteria criteria)
        {
            IList<ReportOvertimeHRDetail> report = null;
            if (criteria != null && criteria.IsValid())
            {
                report = overtimeRepository.GetReportOvertimeHRDetail(criteria);
            }

            return report ?? new List<ReportOvertimeHRDetail>();
        }

        public void SendMailForTransferNexBalance(string subject, OvertimeConversionCreateViewModel model)
        {
            GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = model.EmpId
            };
            var receiver = baseService.GetDataEntity<Employee>(getEmployeeCriteria);

            var templatePath = Path.Combine(environment.WebRootPath, Constants.EmailTemplatePathForOvertimeConversion);
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

        public void SendMailForNEXRequest(string subject, CreateOvertimeViewModel model)
        {
            if (model != null)
            {
                IList<Employee> managers = baseService.GetData<Employee>(new GetManagementsCriteria()).ToList();
                var templatePath = Path.Combine(environment.WebRootPath, Constants.EmailTemplatePathForOvertimeRequest);
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

        public void SendMailForApprovalNotification(string subject, int overtimeId, int createdBy, int approvedBy)
        {
            GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = approvedBy
            };
            var manager = baseService.GetDataEntity<Employee>(getEmployeeCriteria);
            var templatePath = Path.Combine(environment.WebRootPath, Constants.EmailTemplatePathForOvertimeApprovalNotification);
            var bodyTemplate = Utilities.ReadTextFile(templatePath);

            var criteria = new GetOvertimeByIdCriteria
            {
                Id = overtimeId
            };
            var overtime = overtimeRepository.GetOvertimeById(criteria);
            var emloyees = string.Join(
                                               Constants.BrTag,
                                               overtime.OvertimeRecords.Select(t =>
                                                                string.Format(
                                                                    Constants.ContentEmployeeOvertime,
                                                                    t.Employee.NickName,
                                                                    t.Employee.FullName,
                                                                    t.Date.ToString(FormatDate),
                                                                    t.Hour)));
            SendMailForCreatedRequest(subject, createdBy, manager.NickName, bodyTemplate, emloyees, overtime.Description);
            SendMailForHR(subject, createdBy, manager.NickName, bodyTemplate, emloyees, overtime.Description);
        }

        private void SendMailForCreatedRequest(string subject, int createdBy, string managerNickName, string bodyTemplate, string employees, string project)
        {
            GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = createdBy
            };
            var receiver = baseService.GetDataEntity<Employee>(getEmployeeCriteria);
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

        private void SendMailForHR(string subject, int createdBy, string managerNickName, string bodyTemplate, string employees, string project)
        {
            IList<Employee> HRs = baseService.GetData<Employee>(new GetHRCriteria()).ToList();
            GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = createdBy
            };
            var creator = baseService.GetDataEntity<Employee>(getEmployeeCriteria);
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
                    string.Join(";", HRs.Select(t => t.Email)),
                    string.Empty,
                    subject,
                    body);
        }

        public void SendMailForNotification(string subject, OvertimeVerifyViewModel model)
        {
            var templatePath = Path.Combine(environment.WebRootPath, Constants.EmailTemplatePathForOvertimeNotification);
            var bodyTemplate = Utilities.ReadTextFile(templatePath);

            foreach (var item in model.TimeSheets)
            {
                var employee = baseService.GetDataEntity<Employee>(new GetEmployeeByIdCriteria
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