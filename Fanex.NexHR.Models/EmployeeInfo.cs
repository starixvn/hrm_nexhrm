namespace Fanex.NexHR.Models
{
    using System;

    public class EmployeeInfo
    {
        public string DomainUsername { get; set; }

        public string FullName { get; set; }

        public string EnglishName { get; set; }

        public string Code { get; set; }

        public string Email { get; set; }

        public string EmployeeCard { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PlaceOfBirth { get; set; }

        public string JobTitle { get; set; }

        public string JobGrade { get; set; }

        public string Phone { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public string IdCardNumber { get; set; }
        
        public DateTime? IssuedDate { get; set; }

        public string IssuedPlace { get; set; }

        public string Gender { get; set; }

        public DateTime? JoinedDate { get; set; }

        public DateTime? ConfirmedDate { get; set; }

        public DateTime? ResignedDate { get; set; }

        public string PermanentAddress { get; set; }

        public string CurrentAddress { get; set; }

        public string PassportNo { get; set; }

        public DateTime? PassportExp { get; set; }

        public string SocialInsuranceNo { get; set; }

        public bool Enabled { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsSpecialUser { get; set; }

        public string Skype { get; set; }

        public string Telegram { get; set; }

        public bool Marriage { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string SupervisorEnglishName { get; set; }

        public string SupervisorFullName { get; set; }

        public string SupervisorEmail { get; set; }
    }
}
