using System;

namespace Fanex.NexHR.Models
{
    public class TrainingAndDevelopment
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string ConductedBy { get; set; }
        public string Description { get; set; }
        public string TeamName { get; set; }
        public string TeamCode { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExamDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Organization { get; set; }
    }
}
