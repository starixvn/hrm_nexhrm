namespace Fanex.NexHR.Criteria.Applicant
{
    using System;
    using Core.Attributes;

    public class UpdateApplicantCriteria : BaseCommand
    {
        [Output]
        public int Id { get; set; }
        public string FullName { get; set; }

        public int PositionId { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public int? Status { get; set; }

        public DateTime? DOB { get; set; }

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

        public string EducationBackground { get; set; }

        public int? Grade { get; set; }

        public string InterviewerComment { get; set; }

        public string Note { get; set; }

        public string InterviewerIds { get; set; }

        public string OfferSalary { get; set; }

        public string ExpectedSalary { get; set; }

        public string FinalComment { get; set; }

        public override bool IsValid()
        {
            return  IsEmployeeIdAndPositionIdValid()
                && !string.IsNullOrWhiteSpace(FullName)
                && !string.IsNullOrWhiteSpace(Phone)
                && !string.IsNullOrWhiteSpace(Email);
        }

        private bool IsEmployeeIdAndPositionIdValid()
        {
            return Id > 0 && PositionId > 0;
        }

        public override string GetSettingKey()
        {
            return "Applicant_Update";
        }
    }
}