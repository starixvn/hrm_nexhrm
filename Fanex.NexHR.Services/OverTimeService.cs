namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using Criteria.Overtime;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Resources;
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

        public void SendMail(string subject, string memoId)
        {
            var link = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath;
            var groupSd = ConfigurationManager.AppSettings["EmailSD"].ToString();
            var groupGo = ConfigurationManager.AppSettings["EmailGo"].ToString();
            var groupNetwork = ConfigurationManager.AppSettings["EmailNetwork"].ToString();
            //// send email(s)
            var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/MemoNotification.cshtml"));

            var body = string.Format(bodyTemplate, subject, link + "memo/detail/" + memoId);
            this.emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                groupGo,
                string.Empty,
                Messages.MemoSubject + " " + subject,
                body);
            this.emailService.SendEmail(
               ConfigurationManager.AppSettings["Email"],
               groupSd,
               string.Empty,
               Messages.MemoSubject + " " + subject,
               body);
            this.emailService.SendEmail(
               ConfigurationManager.AppSettings["Email"],
               groupNetwork,
               string.Empty,
               Messages.MemoSubject + " " + subject,
               body);
        }
    }
}
