namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Models.RedmineAPI;
    using Models.Request;

    public class ToDoViewModel
    {
        public IList<MissingCheckInOutModel> MissingCheckInOutList { get; set; }

        public IList<RequestEntity> PendingRequestsList { get; set; }

        public IList<RedmineIssue> InProgressTasks { get; set; }

        public IList<DateTime> ForgotLogtimeDates { get; set; }
    }
}