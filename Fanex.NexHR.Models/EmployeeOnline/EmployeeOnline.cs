namespace Fanex.NexHR.Models.EmployeeOnline
{
    using System;
    public class EmployeeOnline
    {
        public int EmpId { get; set; }

        public string NickName { get; set; }

        public DateTime CheckingTime { get; set; }

        public bool IsOnline { get; set; }

        public string PCName { get; set; }
    }
}
