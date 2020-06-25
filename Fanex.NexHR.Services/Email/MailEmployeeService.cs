namespace Fanex.NexHR.Services.Email
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using log4net;
    using Fanex.NexHR.Resources;

    public class MailEmployeeService
    {
        private readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string HRMName = ConfigurationManager.AppSettings["HRMName"];

        private readonly EmailService EmailService = new EmailService();

        private readonly Employee _employee;

        public MailEmployeeService(Employee employee)
        {
            _employee = employee;
        }

        public void SendMailUpdateSuccess()
        {
            var bodyTemplate = ReadTextFile(Constants.EditEmployeeEmailTemplate);
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

        private string ReadTextFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var pathCombine = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

            if (!File.Exists(pathCombine))
            {
                throw new FileNotFoundException(pathCombine);
            }

            try
            {
                using (var sr = new StreamReader(pathCombine))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return string.Empty;
            }
        }
    }
}