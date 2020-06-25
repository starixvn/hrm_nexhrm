namespace Fanex.NexHR.Services
{
    using Core;
    using Interfaces;
    using MailDeliver.MailClient.API;
    using MailDeliver.Model;
    using System.Configuration;

    public class EmailService : IEmailService
    {
        public void SendEmail(string senderEmail, string receiverEmail, string mailCC, string subject, string body)
        {
            var disableSendMail = bool.Parse(ConfigurationManager.AppSettings["IsDisabledSendMail"]);

            if (!Utilities.IsValidEmail(senderEmail)
                || !Utilities.IsValidEmail(receiverEmail)
                || disableSendMail)
            {
                return;
            }

            var mailItem = new MailRecord
            {
                MailTo = receiverEmail,
                MailCc = mailCC,
                Subject = subject,
                Title = $"[{ConfigurationManager.AppSettings["HRMName"]}]",
                Body = body,
            };

            using (var client = new MailDeliver())
            {
                client.SendMail(mailItem);
            }
        }
    }
}