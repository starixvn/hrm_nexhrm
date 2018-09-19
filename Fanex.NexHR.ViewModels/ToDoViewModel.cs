namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Models.RedmineAPI;
    using Models.Request;
    using Models.Report;

    public class ToDoViewModel
    {
        public ToDoViewModel()
        {
            MissingCheckInOutList = new List<MissingCheckInOutModel>();
            PendingRequestsList = new List<RequestEntity>();
            InProgressTasks = new List<RedmineIssue>();
            ForgotLogtimeDates = new List<DateTime>();
            EmployeeLatetInEarlyOuts = new List<EmployeeLateInEarlyOut>();
        }

        public IList<MissingCheckInOutModel> MissingCheckInOutList { get; set; }

        public IList<RequestEntity> PendingRequestsList { get; set; }

        public IList<RedmineIssue> InProgressTasks { get; set; }

        public IList<DateTime> ForgotLogtimeDates { get; set; }

        public bool IsAllowInputSpendTime { get; set; }

        public IList<EmployeeLateInEarlyOut> EmployeeLatetInEarlyOuts { get; set; }
    }
}