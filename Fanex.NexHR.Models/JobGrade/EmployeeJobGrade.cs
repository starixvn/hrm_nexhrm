using System;

namespace Fanex.NexHR.Models
{
    public class EmployeeJobGrade
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public int GroupId { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string TeamName { get; set; }
        public string TeamCode { get; set; }
    
    }
}
