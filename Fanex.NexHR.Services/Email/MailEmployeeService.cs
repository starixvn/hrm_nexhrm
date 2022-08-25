namespace Fanex.NexHR.Services.Email
{
    using System.IO;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Core;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Hosting;

    public class MailEmployeeService : IMailEmployeeService
    {
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment environment;
        private readonly AppSetting appSetting;
        private readonly EmailSettings emailSettings;

        public MailEmployeeService(IEmailService emailService, IWebHostEnvironment environment, AppSetting appSetting, EmailSettings emailSettings)
        {
            this.emailService = emailService;
            this.environment = environment;
            this.appSetting = appSetting;
            this.emailSettings = emailSettings;
        }

        public void SendMailUpdateSuccess(Employee employee)
        {
            var templatePath = Path.Combine(environment.ContentRootPath, Constants.EmailTemplatePathForSuccessUpdateEmployee);
            var bodyTemplate = Utilities.ReadTextFile(templatePath);
            var emailContent = string.Format(
                bodyTemplate,
                employee.NickName,
                appSetting.HRMName,
                appSetting.BaseUrl,
                emailSettings.CompanyMailSign);
            var emailSubject = $"[{appSetting.HRMName}] - {Messages.SuccessUpdateInformation}";

            emailService.SendEmail(
                emailSettings.Email,
                employee.Email,
                null,
                emailSubject,
                emailContent);
        }
    }
}