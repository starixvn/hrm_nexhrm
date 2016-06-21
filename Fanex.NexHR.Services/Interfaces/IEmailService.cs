// --------------------------------------------------------------------------
// <copyright file="IEmailService.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(string senderEmail, string receiverEmail, string mailCC, string subject, string body);
    }
}
