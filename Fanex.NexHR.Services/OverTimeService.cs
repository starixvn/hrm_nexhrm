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

        public void SendMail(string subject, CreateOvertimeViewModel model)
        {
            if (model != null)
            {
                GetLeaderAndHRCriteria getLeaderCriteria = new GetLeaderAndHRCriteria();
                GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria();

                //// send email(s)
                var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForOvertimeNotification));

                foreach (var item in model.TimeSheets)
                {
                    getLeaderCriteria.EmployeeId = item.Employee.Id;
                    getEmployeeCriteria.EmployeeId = item.Employee.Id;
                    var leaderReceive = GetData<Employee>(getLeaderCriteria);
                    var receiver = GetDataEntity<Employee>(getEmployeeCriteria);

                    var body = string.Format(
                        bodyTemplate,
                        item.Employee.NickName,
                        ConfigurationManager.AppSettings["OTRequest"],
                        model.Date.ToDateMonthYearFormatedDate(),
                        item.Hour,
                        model.Description,
                        ConfigurationManager.AppSettings["CompanyMailSign"]);

                    _emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        receiver.Email,
                        string.Join(";", leaderReceive.Select(r => r.Email)),
                        subject,
                        body);
                }
            }
        }
    }
}
