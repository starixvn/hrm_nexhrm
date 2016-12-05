namespace Fanex.NexHR.Services
{
    using System.Configuration;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services.Interfaces;

    public class EmailService : IEmailService
    {
        public void SendEmail(string senderEmail, string receiverEmail, string mailCC, string subject, string body)
        {
            var disableSendMail = bool.Parse(ConfigurationManager.AppSettings["IsDisabledSendMail"].ToString());
            if (!Utilities.IsValidEmail(senderEmail) ||
                !Utilities.IsValidEmail(receiverEmail)
                || disableSendMail)
            {
                return;
            }

            var mailItem = new MailRecord
            {
                MailTo = receiverEmail,
                MailCc = mailCC,
                Subject = subject,
                Title = ConfigurationManager.AppSettings["Email.SenderName"],
                Body = body,
            };

            using (var client = new MailDeliver())
            {
                client.SendMail(mailItem);
            }
        }
    }
}
