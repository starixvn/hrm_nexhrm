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

        public IEnumerable<MissingCheckInOutModel> MissingCheckInOutList { get; set; }

        public IEnumerable<RequestEntity> PendingRequestsList { get; set; }

        public IEnumerable<RedmineIssue> InProgressTasks { get; set; }

        public IEnumerable<DateTime> ForgotLogtimeDates { get; set; }

        public bool IsAllowInputSpendTime { get; set; }

        public IEnumerable<EmployeeLateInEarlyOut> EmployeeLatetInEarlyOuts { get; set; }
    }
}