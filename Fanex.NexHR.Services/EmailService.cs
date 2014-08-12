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
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// Email service.
    /// </summary>
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="senderEmail">The sender email.</param>
        /// <param name="receiverEmail">The receiver email.</param>
        /// <param name="mailCC">The CC list.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="body">The email body.</param>
        public void SendEmail(string senderEmail, string receiverEmail, string mailCC, string subject, string body)
        {
            this.SendEmail(senderEmail, senderEmail, receiverEmail, mailCC, subject, body);
        }

        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="senderEmail">The sender email.</param>
        /// <param name="senderName">The sender name.</param>
        /// <param name="receiverEmail">The receiver email.</param>
        /// <param name="mailCC">The CC list.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="body">The email body.</param>
        public void SendEmail(
            string senderEmail, 
            string senderName, 
            string receiverEmail, 
            string mailCC, 
            string subject, 
            string body)
        {
            if (!Utilities.IsValidEmail(senderEmail) ||
                !Utilities.IsValidEmail(receiverEmail))
            {
                return;
            }

            var mailItem = new MailRecord
            {
                MailTo = receiverEmail,
                MailCc = mailCC,
                Subject = subject,
                Title = ConfigurationManager.AppSettings["Email.SenderName"],
                Body = body
            };

            using (var client = new MailDeliver())
            {
                client.SendMail(mailItem);
            }
        }
    }
}
