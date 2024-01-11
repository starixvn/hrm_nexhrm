namespace Fanex.NexHR.ViewModels.HistoryWork
{
     using System;
    using Models;

    public class FilterHistoryWorkViewModel
    {

        public FilterHistoryWorkViewModel()
        {
            ToDate = DateTimeOffset.Now.DateTime;
        }
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? EmpId { get; set; }

        public Employee Employee { get; set; }
    }
}
