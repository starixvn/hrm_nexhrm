namespace Fanex.NexHR.Models.Calendar
{
    using System;

    public class WorkingSchedule
    {
        public int EmpId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string DepartmentName { get; set; }

        public string TeamName { get; set; }

        public string TeamCode { get; set; }

        public DateTime WorkDate { get; set; }

        public bool IsLeaveHalfDay { get; set; }

        public string Title { get; set; }
    }
}
