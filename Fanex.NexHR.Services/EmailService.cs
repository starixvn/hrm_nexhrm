// --------------------------------------------------------------------------
// <copyright file="EmailService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.MailDeliver.Model;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services.Interface;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    public class EmailService : IEmailService
    {
        public void SendEmail(string senderEmail, string receiverEmail, string mailCc, string subject, string body)
        {
            this.SendEmail(senderEmail, senderEmail, receiverEmail, mailCc, subject, body);
        }

        public void SendEmail(string senderEmail, string senderName, string receiverEmail, string mailCc,  string subject, string body)
        {
            if (!Utilities.IsValidEmail(senderEmail) ||
                !Utilities.IsValidEmail(receiverEmail))
            {
                return;
            }

            var mailItem = new MailRecord
            {
                MailTo = receiverEmail,
                MailCc = mailCc,
                Subject = subject,
                Title = ConfigurationManager.AppSettings["Email.SenderName"],
                Body = body
            };
            using (var client = new Fanex.MailDeliver.MailClient.API.MailDeliver())
            {
                client.SendMail(mailItem);
            }
        }
    }
}
