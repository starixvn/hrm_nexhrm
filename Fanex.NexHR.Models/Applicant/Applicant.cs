namespace Fanex.NexHR.Models.Applicant
{
    using System;

    public class Applicant
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public int Phone { get; set; }

        public string Email { get; set; }

        public string Position { get; set; }

        public DateTime InterviewDate1 { get; set; }

         public DateTime InterviewDate2 { get; set; }

         public string Notes { get; set; }

         public string Source { get; set; }

         public string CV { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee ModifiedBy { get; set; }
    }
}
