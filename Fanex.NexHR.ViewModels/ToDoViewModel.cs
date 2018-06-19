namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Models.RedmineAPI;
    using Models.Request;

    public class ToDoViewModel
    {
        public ToDoViewModel()
        {
            MissingCheckInOutList = new List<MissingCheckInOutModel>();
            PendingRequestsList = new List<RequestEntity>();
            InProgressTasks = new List<RedmineIssue>();
            ForgotLogtimeDates = new List<DateTime>();
        }

        public IList<MissingCheckInOutModel> MissingCheckInOutList { get; set; }

        public IList<RequestEntity> PendingRequestsList { get; set; }

        public IList<RedmineIssue> InProgressTasks { get; set; }

        public IList<DateTime> ForgotLogtimeDates { get; set; }
    }
}