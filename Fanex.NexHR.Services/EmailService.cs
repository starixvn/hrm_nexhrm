namespace Fanex.NexHR.Services
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Core;
    using Fanex.Logging;
    using Fanex.NexHR.Models.Core;
    using Interfaces;

    public class EmailService : IEmailService
    {
        private readonly ILogServiceProvider logServiceProvider;
        private readonly EmailSettings emailSettings;
        private readonly AppSetting appSetting;

        public EmailService(ILogServiceProvider logServiceProvider, EmailSettings emailSettings, AppSetting appSetting)
        {
            this.logServiceProvider = logServiceProvider;
            this.emailSettings = emailSettings;
            this.appSetting = appSetting;
        }

        public void SendEmail(string senderEmail, string receiverEmail, string mailCC, string subject, string body)
        {
            if (!Utilities.IsValidEmail(senderEmail)
                || !Utilities.IsValidEmail(receiverEmail)
                || emailSettings.IsDisabledSendMail)
            {
                return;
            }

            try
            {
                var client = new SmtpClient(emailSettings.Host, emailSettings.Port);
                var msg = new MailMessage();

                msg.From = new MailAddress(emailSettings.Email, $"[{appSetting.HRMName}]");
                GetMailFromString(receiverEmail, msg.To);
                GetMailFromString(mailCC, msg.CC);
                msg.Subject = subject;
                msg.SubjectEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.Body = body;
                msg.BodyEncoding = Encoding.UTF8;
                msg.Priority = MailPriority.Normal;

                ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                client.SendAsync(msg, null);

                logServiceProvider.LogFile(String.Format("To: {0} - CC: {1} - Subject: {2}", msg.To, msg.CC, subject));
            }
            catch (Exception ex)
            {
                logServiceProvider.LogFile("==========================Email error==============================");
                logServiceProvider.LogFile(ex.Message);
            }
        }

        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return policyErrors == SslPolicyErrors.None;
        }

        private static void GetMailFromString(string mailString, MailAddressCollection mails)
        {
            if (!string.IsNullOrWhiteSpace(mailString))
            {
                var emails = RemoveDoubleComma(mailString);
                var mailArrs = emails.Split(';');
                foreach (var mail in mailArrs)
                {
                    if (!string.IsNullOrEmpty(mail))
                    {
                        mails.Add(mail);
                    }
                }
            }
        }

        private static string RemoveDoubleComma(string mails)
        {
            var emails = mails;
            while (emails.Contains(",,"))
            {
                emails = emails.Replace(",,", ",");
            }

            while (emails.Contains(";;"))
            {
                emails = emails.Replace(";;", ";");
            }

            return emails.TrimEnd(',', ';');
        }
    }
}