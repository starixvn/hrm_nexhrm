namespace Fanex.NexHR.Services.Email
{
    using System.Configuration;
    using System.IO;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Hosting;

    public class MailEmployeeService: IMailEmployeeService
    {
        private readonly string HRMName = ConfigurationManager.AppSettings["HRMName"];

        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment environment;

        public MailEmployeeService(IEmailService emailService, IWebHostEnvironment environment)
        {
            this.emailService = emailService;
            this.environment = environment;
        }

        public void SendMailUpdateSuccess(Employee employee)
        {
            var templatePath = Path.Combine(environment.WebRootPath, Constants.EmailTemplatePathForSuccessUpdateEmployee);
            var bodyTemplate = Utilities.ReadTextFile(templatePath);
            var emailContent = string.Format(
                bodyTemplate,
                employee.NickName,
                HRMName,
                ConfigurationManager.AppSettings["BaseUrl"],
                ConfigurationManager.AppSettings["CompanyMailSign"]);
            var emailSubject = $"[{HRMName}] - {Messages.SuccessUpdateInformation}";

            emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                employee.Email,
                null,
                emailSubject,
                emailContent);
        }
    }
}