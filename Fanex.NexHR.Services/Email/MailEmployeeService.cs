namespace Fanex.NexHR.Services.Email
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using log4net;

    public class MailEmployeeService
    {
        private readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string SenderName = ConfigurationManager.AppSettings["Email.SenderName"];

        private readonly string SenderEmail = ConfigurationManager.AppSettings["Email.Sender"];

        private readonly EmailService EmailService = new EmailService();

        private readonly Employee _employee;

        public MailEmployeeService(Employee employee)
        {
            _employee = employee;
        }

        public void SendMailUpdateSuccess()
        {
            var bodyTemplate = ReadTextFile("emails\\successupdate.cshtml");

            if (string.IsNullOrWhiteSpace(bodyTemplate))
            {
                Log.Error("Cannot load email template service emails/successupdate.cshtml or it's empty");
                return;
            }

            var emailContent = string.Format(bodyTemplate, _employee.NickName, DateTime.Now.ToShortDateString(), "Successful Updating");
            var emailSubject = $"{SenderName} - Success update infomation";

            EmailService.SendEmail(
                SenderEmail,
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