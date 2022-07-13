namespace Fanex.NexHR.Services
{
    using Core;
    using Fanex.NexHR.Models.Core;
    using Interfaces;
    using MailDeliver.MailClient.API;
    using MailDeliver.Model;

    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;
        private readonly AppSetting appSetting;

        public EmailService(EmailSettings emailSettings, AppSetting appSetting)
        {
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

            var mailItem = new MailRecord
            {
                MailTo = receiverEmail,
                MailCc = mailCC,
                Subject = subject,
                Title = $"[{appSetting.HRMName}]",
                Body = body,
            };

            using (var client = new MailDeliver())
            {
                client.SendMail(mailItem);
            }
        }
    }
}