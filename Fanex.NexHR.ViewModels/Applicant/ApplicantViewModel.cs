namespace Fanex.NexHR.ViewModels.Applicant
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models;

    public class ApplicantViewModel
    {

        public ApplicantViewModel()
        {
            this.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.ToDate = DateTime.Now;
        }

        [Required]
        public string FullName { get; set; }

        [Required]
        public int PositionId { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        public int Status { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IEnumerable<Position> Positions { get; set; }

        public DateTime DOB { get; set; }

        public DateTime? ApplyDate { get; set; }

        public int YearsOfExperience { get; set; }

        public int ProfessionalLevel { get; set; }

        public string ProfessionalMajor { get; set; }

        public DateTime InterviewDate1 { get; set; }

        public int Result1 { get; set; }

        public DateTime InterviewDate2 { get; set; }

        public int Result2 { get; set; }

        public DateTime JoinDate { get; set; }

        public int Source { get; set; }

        public int Recruiter { get; set; }

        public string EducationBackground { get; set; }

        public int Grade { get; set; }

        public string InterviewerComment { get; set; }

        public string Note { get; set; }

        public int InterviewerId { get; set; }
        
        public string OfferSalary { get; set; }

        public string ExpectedSalary { get; set; }

        public string FinalComment { get; set; }
    }
}