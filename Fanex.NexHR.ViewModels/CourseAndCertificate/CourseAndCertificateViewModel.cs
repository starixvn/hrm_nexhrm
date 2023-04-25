namespace Fanex.NexHR.ViewModels.CourseAndCertificate
{
    using System;

    public class CourseAndCertificateViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Employee { get; set; }
        public string Department { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExamDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
