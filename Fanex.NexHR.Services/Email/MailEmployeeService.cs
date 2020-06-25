namespace Fanex.NexHR.Services.Email
{
    using System.Configuration;
    using System.IO;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;

    public class MailEmployeeService
    {
        private readonly string HRMName = ConfigurationManager.AppSettings["HRMName"];

        private readonly EmailService EmailService = new EmailService();

        private readonly Employee _employee;

        public MailEmployeeService(Employee employee)
        {
            _employee = employee;
        }

        public void SendMailUpdateSuccess()
        {
            var templatePath = Path.Combine(HttpRuntime.AppDomainAppPath, Constants.EmailTemplatePathForSuccessUpdateEmployee);
            var bodyTemplate = Utilities.ReadTextFile(templatePath);
            var emailContent = string.Format(
                bodyTemplate,
                _employee.NickName,
                HRMName,
                ConfigurationManager.AppSettings["BaseUrl"],
                ConfigurationManager.AppSettings["CompanyMailSign"]);
            var emailSubject = $"[{HRMName}] - {Messages.SuccessUpdateInformation}";

            EmailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                _employee.Email,
                null,
                emailSubject,
                emailContent);
        }
    }
}