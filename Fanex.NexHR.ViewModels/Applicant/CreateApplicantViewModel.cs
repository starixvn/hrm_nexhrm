namespace Fanex.NexHR.ViewModels.Applicant
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateApplicantViewModel
    {

        public CreateApplicantViewModel()
        {
            DOB = new DateTime(1990, 1, 1);
            ApplyDate = DateTime.Now;
        }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Position")]
        public int PositionId { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        public int? Status { get; set; }

        public IEnumerable<Position> Positions { get; set; }

        public  List<SelectListItem> StatusList { get; set; }

         public  List<SelectListItem> ProLevelList => new List<SelectListItem>
        {
            new SelectListItem {Text = "", Value = ""},
            new SelectListItem {Text = "Fresher", Value = "1"},
            new SelectListItem {Text = "Engineer", Value = "2"},
            new SelectListItem {Text = "Senior", Value = "3"},
            new SelectListItem {Text = "Uper Senior", Value = "4"}
        };

        public DateTime DOB { get; set; }

        public DateTime ApplyDate { get; set; }

        public int? YearsOfExperience { get; set; }

        public int? ProfessionalLevel { get; set; }

        public string ProfessionalMajor { get; set; }

         public string Resume { get; set; }

        public DateTime? InterviewDate1 { get; set; }

        public int? Result1 { get; set; }

        public DateTime? InterviewDate2 { get; set; }

        public int? Result2 { get; set; }

        public DateTime? JoinDate { get; set; }

        public int? Source { get; set; }

        public int? RecruiterId { get; set; }

        public string Recruiter { get; set; }

        public string EducationBackground { get; set; }

        public int? Grade { get; set; }

        public string InterviewerComment { get; set; }

        public string Note { get; set; }

        public int? InterviewerIds { get; set; }

        public string OfferSalary { get; set; }

        public string ExpectedSalary { get; set; }

        public string FinalComment { get; set; }

        public string Interviewer { get; set; }

        public int InterviewerId { get; set; }
    }
}