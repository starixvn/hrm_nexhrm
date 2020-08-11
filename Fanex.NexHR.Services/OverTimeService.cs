namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using Core;
    using Criteria.Employee;
    using Criteria.Overtime;
    using Interfaces;
    using Models;
    using Models.Overtime;
    using Repositories;
    using Repositories.Interfaces;
    using ViewModels.Overtime;

    public class OvertimeService : BaseService, IOvertimeService
    {
        private readonly IOvertimeRepository _repository;
        private readonly IEmailService _emailService;

        public OvertimeService()
            : this(null, null)
        {
        }

        public OvertimeService(IOvertimeRepository repository, IEmailService emailService)
        {
            _repository = repository ?? new OvertimeRepository();
            _emailService = emailService ?? new EmailService();
        }

        public IList<Overtime> GetOvertime(GetOvertimeCriteria criteria)
        {
            IList<Overtime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = _repository.GetOvertime(criteria);
            }

            return overtimes ?? new List<Overtime>();
        }

        public Overtime GetOvertimeById(GetOvertimeByIdCriteria criteria)
        {
            Overtime overtime = null;
            if (criteria != null && criteria.IsValid())
            {
                overtime = _repository.GetOvertimeById(criteria);
            }

            return overtime;
        }

        public IList<EmployeeOvertime> GetEmployeeOvertime(GetEmployeeOvertimeCriteria criteria)
        {
            IList<EmployeeOvertime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = _repository.GetEmployeeOvertime(criteria);
            }

            return overtimes ?? new List<EmployeeOvertime>();
        }

        public IList<EmployeeOvertime> GetMyOvertime(GetMyOvertimeCriteria criteria)
        {
            IList<EmployeeOvertime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = _repository.GetEmployeeOvertime(criteria);
            }

            return overtimes ?? new List<EmployeeOvertime>();
        }

        public List<OvertimeSummary> GetOvertimeSummary(OvertimeSummaryCriteria criteria)
        {
            List<OvertimeSummary> listSummary = new List<OvertimeSummary>();
            if (criteria != null && criteria.IsValid())
            {
                listSummary = _repository.GetOvertimeSummary(criteria).ToList();
            }

            return listSummary;
        }

        public IList<OvertimeConversion> GetConversions(GetOvertimeConversionsCriteria criteria)
        {
            IList<OvertimeConversion> conversions = null;
            if (criteria != null && criteria.IsValid())
            {
                conversions = _repository.GetConversions(criteria);
            }

            return conversions ?? new List<OvertimeConversion>();
        }

        public NexHour GetNexHour(GetNexHourCriteria criteria)
        {
            return GetDataEntity<NexHour>(criteria);
        }

        public IList<ReportOvertimeHR> GetReportOvertimeHR(GetReportOvertimeHRCriteria criteria)
        {
            IList<ReportOvertimeHR> report = null;
            if (criteria != null && criteria.IsValid())
            {
                report = _repository.GetReportOvertimeHR(criteria);
            }

            return report ?? new List<ReportOvertimeHR>();
        }

        public void SendMailForTransferNexBalance(string subject, OvertimeConversionCreateViewModel model)
        {
            GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = model.EmpId
            };
            var receiver = GetDataEntity<Employee>(getEmployeeCriteria);
            var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForOvertimeConversion));
            var body = string.Format(
                    bodyTemplate,
                    receiver.NickName,
                    model.DeductDate.ToDateMonthYearFormatedDate(),
                    model.Deduct,
                    model.Description,
                    ConfigurationManager.AppSettings["BaseUrl"],
                    ConfigurationManager.AppSettings["CompanyMailSign"]);

            _emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                    receiver.Email,
                    string.Empty,
                    subject,
                    body);
        }

        public void SendMailForNEXRequest(string subject, CreateOvertimeViewModel model)
        {
            if (model != null)
            {
                IList<Employee> managers = GetData<Employee>(new GetManagementsCriteria()).ToList();
                var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForOvertimeRequest));
                if(managers.Any())
                {
                    var body = string.Format(
                        bodyTemplate,
                        model.CreatedByName,
                        model.Date.ToDateMonthYearFormatedDate(),
                        model.Description,
                        ConfigurationManager.AppSettings["BaseUrl"],
                        ConfigurationManager.AppSettings["CompanyMailSign"]);

                    _emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        string.Join(";", managers.Select(t => t.Email)),
                        string.Empty,
                        subject,
                        body);
                }
            }
        }

        public void SendMailForNotification(string subject, UpdateNexHour model)
        {
            if (model != null)
            {
                GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria
                {
                    EmployeeId = model.EmployeeId
                };
                var receiver = GetDataEntity<Employee>(getEmployeeCriteria);

                //// send email(s)
                var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForOvertimeNotification));

                var body = string.Format(
                    bodyTemplate,
                    receiver.NickName,
                    model.Date.ToDateMonthYearFormatedDate(),
                    model.Hour,
                    model.AdjustHour,
                    model.Description,
                    ConfigurationManager.AppSettings["BaseUrl"],
                    ConfigurationManager.AppSettings["CompanyMailSign"]);

                _emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                    receiver.Email,
                    string.Empty,
                    subject,
                    body);
            }
        }

        public void SendMailForApprovalNotification(string subject, int createdBy, int approvedBy)
        {
            GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = approvedBy
            };
            var manager = GetDataEntity<Employee>(getEmployeeCriteria);

            SendMailForCreatedRequest(subject, createdBy, manager.DisplayName);
            SendMailForHR(subject, manager.DisplayName);
        }

        private void SendMailForCreatedRequest(string subject, int createdBy, string managerDisplayName)
        {
            GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = createdBy
            };
            var receiver = GetDataEntity<Employee>(getEmployeeCriteria);
            var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForOvertimeApprovalNotification));
            var body = string.Format(
                    bodyTemplate,
                    receiver.NickName,
                    "your created NEX request",
                    managerDisplayName,
                    ConfigurationManager.AppSettings["BaseUrl"],
                    ConfigurationManager.AppSettings["CompanyMailSign"]);

            _emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                        receiver.Email,
                        string.Empty,
                        subject,
                        body);
        }

        private void SendMailForHR(string subject, string managerDisplayName)
        {
            IList<Employee> employees = GetData<Employee>(new GetHRCriteria()).ToList();
            var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForOvertimeApprovalNotification));
            var body = string.Format(
                    bodyTemplate,
                    "a NEX request",
                    managerDisplayName,
                    ConfigurationManager.AppSettings["BaseUrl"],
                    ConfigurationManager.AppSettings["CompanyMailSign"]);

            _emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                        string.Join(";", employees.Select(t => t.Email)),
                        string.Empty,
                        subject,
                        body);
        }

        
    }
}
