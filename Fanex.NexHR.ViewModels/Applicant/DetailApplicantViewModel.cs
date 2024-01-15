namespace Fanex.NexHR.ViewModels.Applicant
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class DetailApplicantViewModel
    {

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public string Position { get; set; }

        public DateTime DOB { get; set; }

        public DateTime ApplyDate { get; set; }

        public int? YearsOfExperience { get; set; }

        public string ProfessionalLevel { get; set; }

        public string ProfessionalMajor { get; set; }

         public string Resume { get; set; }

        public DateTime? InterviewDate1 { get; set; }

        public string Result1 { get; set; }

        public DateTime? InterviewDate2 { get; set; }

        public string Result2 { get; set; }

        public DateTime? JoinDate { get; set; }

        public string Source { get; set; }

        public string Recruiter { get; set; }
        
        public string FullNameRecruiter { get; set; }

        public string NickNameRecruiter { get; set; }

        public string EducationBackground { get; set; }

        public string Grade { get; set; }

        public string InterviewerComment { get; set; }

        public string Note { get; set; }

        public string OfferSalary { get; set; }

        public string ExpectedSalary { get; set; }

        public string FinalComment { get; set; }

        public string Interviewer { get; set; }
    }
}