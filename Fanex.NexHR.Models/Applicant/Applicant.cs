namespace Fanex.NexHR.Models.Applicant
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Core;

    public class Applicant
    {
        private DateTime? serverInterviewDate1;
        private DateTime? serverInterviewDate2;

        public int Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string EducationBackground { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        public string Position { get; set; }

        public string PositionCode { get; set; }

        [DataType(DataType.Date)]
        public DateTime? InterviewDate1
        {
            get => serverInterviewDate1;
            set => serverInterviewDate1 = value.HasValue ? value.Value.ToClientTimeZone() : null;
        }

        [DataType(DataType.Date)]
        public DateTime? InterviewDate2
        {
            get => serverInterviewDate2;
            set => serverInterviewDate2 = value.HasValue ? value.Value.ToClientTimeZone() : null;
        }

        [DataType(DataType.Date)]
        public DateTime? JoinDate { get; set; }

        public string Status { get; set; }

        public string Note { get; set; }

        public string Source { get; set; }

        public string Result1 { get; set; }

        public string Result2 { get; set; }

        public string Resume { get; set; }

        public bool IsOnTop { get; set; }

        public byte? GradeId { get; set; }

        public DateTime ApplyDate { get; set; }

        public float? YearsOfExperience { get; set; }
    }
}
