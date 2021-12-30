namespace Fanex.NexHR.Models.EmployeeOnline
{
    using System;
    using Fanex.NexHR.Core.XML;

    [Serializable]
    [XMLName("EmployeeOnline")]
    public class EmployeeOnlineXML
    {
        public int EmpId { get; set; }

        public DateTime CheckingTime { get; set; }

        public bool IsOnline { get; set; }

        public string PCName { get; set; }
    }
}
