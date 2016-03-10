namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using Criteria.Overtime;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels.Overtime;
    using Interfaces;
    using Models;
    using Models.Overtime;
    using Repositories;
    using Repositories.Interfaces;

    public class OvertimeService : BaseService, IOvertimeService
    {
        private readonly IOvertimeRepository repository;
        private readonly IEmailService emailService;

        public OvertimeService()
            : this(null, null)
        {
        }

        public OvertimeService(IOvertimeRepository repository, IEmailService emailService)
        {
            this.repository = repository ?? new OvertimeRepository();
            this.emailService = emailService ?? new EmailService();
        }

        public IList<Overtime> GetOvertime(GetOvertimeCriteria criteria)
        {
            IList<Overtime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = repository.GetOvertime(criteria);
            }

            return overtimes ?? new List<Overtime>();
        }

        public Overtime GetOvertimeById(GetOvertimeByIdCriteria criteria)
        {
            Overtime overtime = null;
            if (criteria != null && criteria.IsValid())
            {
                overtime = repository.GetOvertimeById(criteria);
            }

            return overtime;
        }

        public IList<EmployeeOvertime> GetEmployeeOvertime(GetEmployeeOvertimeCriteria criteria)
        {
            IList<EmployeeOvertime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = repository.GetEmployeeOvertime(criteria);
            }

            return overtimes ?? new List<EmployeeOvertime>();
        }

        public IList<EmployeeOvertime> GetMyOvertime(GetMyOvertimeCriteria criteria)
        {
            IList<EmployeeOvertime> overtimes = null;
            if (criteria != null && criteria.IsValid())
            {
                overtimes = repository.GetEmployeeOvertime(criteria);
            }

            return overtimes ?? new List<EmployeeOvertime>();
        }

        public List<OvertimeSummary> GetOvertimeSummary(OvertimeSummaryCriteria criteria)
        {
            List<OvertimeSummary> listSummary = new List<OvertimeSummary>();
            if (criteria != null && criteria.IsValid())
            {
                listSummary = repository.GetOvertimeSummary(criteria).ToList();
            }

            return listSummary;
        }

        public void SendMail(string subject, CreateOvertimeViewModel model)
        {
            GetLeadersOfEmployeeCriteria getLeaderCriteria = new GetLeadersOfEmployeeCriteria();
            GetEmployeeByIdCriteria getEmployeeCriteria = new GetEmployeeByIdCriteria();

            IList<Employee> leaderReceive;
            Employee receiver;

            //// send email(s)
            var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/OvertimeNotification.cshtml"));

            foreach (var item in model.TimeSheets)
            {
                getLeaderCriteria.EmployeeId = item.Employee.Id;
                getEmployeeCriteria.EmployeeId = item.Employee.Id;
                leaderReceive = GetData<Employee>(getLeaderCriteria);
                receiver = GetDataEntity<Employee>(getEmployeeCriteria);

                var body = string.Format(bodyTemplate, item.Employee.NickName, model.Date.ToDateMonthYearFormatedDate(), item.Hour, model.Description);

                this.emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                    receiver.Email,
                    string.Join(";", leaderReceive.Select(r => r.Email)),
                    subject,
                    body);
            }
        }
    }
}
