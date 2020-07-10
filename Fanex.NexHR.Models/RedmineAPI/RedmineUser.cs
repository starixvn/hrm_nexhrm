namespace Fanex.NexHR.Models.RedmineAPI
{
    using System;
    using System.Collections.Generic;

    public class RedmineUser
    {
        public int RedmineId { get; set; }

        public string Name { get; set; }

        public string ApiKey { get; set; }

        public string Email { get; set; }

        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string ConversationId { get; set; }

        public IList<DateTime> ForgotLogtimeDates { get; set; }
    }
}