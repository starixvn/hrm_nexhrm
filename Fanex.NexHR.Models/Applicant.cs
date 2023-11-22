namespace Fanex.NexHR.Models.Applicant
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Applicant
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Position { get; set; }

        [DataType(DataType.Date)]
        public DateTime? InterviewDate1 { get; set; }

        [DataType(DataType.Date)]
        public DateTime? InterviewDate2 { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JoinDate { get; set; }

        public string Notes { get; set; }

        public int Source { get; set; }

        public string SourceName { get; set; }

        public string CV { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee CreatedBy { get; set; }
    }
}
