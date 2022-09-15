namespace Fanex.NexHR.Services
{
    using Core;
    using Fanex.MailDeliver.MailClient;
    using Fanex.NexHR.Models.Core;
    using Interfaces;
    using MailDeliver.MailClient.API;
    using MailDeliver.Model;
    using System;
    using System.Net.Mail;
    using System.Text;

    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;
        private readonly AppSetting appSetting;

        public EmailService(EmailSettings emailSettings, AppSetting appSetting)
        {
            this.emailSettings = emailSettings;
            this.appSetting = appSetting;
            AppSettings.AppMailAddress = emailSettings.AppMailAddress;
            AppSettings.AppMailId = emailSettings.AppMailId;
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

                var client = new System.Net.Mail.SmtpClient
                {
                    Port = emailSettings.Port,
                    Host = emailSettings.Host
                };
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;

                client.SendAsync(msg, null);
            }
            catch (Exception ex)
            {
                throw;
            }
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

            return emails.TrimEnd(new[] { ',', ';' });
        }
    }
}