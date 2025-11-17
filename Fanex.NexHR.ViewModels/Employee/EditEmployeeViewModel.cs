namespace Fanex.NexHR.ViewModels.Employee
{
    using Core;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EditEmployeeViewModel
    {
        public EditEmployeeViewModel()
        {
            InitData(null);
        }

        public EditEmployeeViewModel(Employee employee)
        {
            InitData(employee);
        }

        public int Id { get; set; }

        public string Code { get; set; }

        [Required]
        public string FullName { get; set; }

        public string NickName { get; set; }

        public string DomainUsername { get; set; }

        public string EmployeeCard { get; set; }

        public string Roles { get; set; }

        public int TeamId { get; set; }

        public string EducationBackground { get; set; }
        public int JobTitleId { get; set; }

        [Display(Name = "Other team(s)")]
        public string SubGroups { get; set; }

        public string Approvers { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string PlaceOfBirth { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public byte? Gender { get; set; }

        public string Skype { get; set; }

        public string Yahoo { get; set; }

        public bool Marriage { get; set; }

        public string Address { get; set; }

        public string PermanentAddress { get; set; }

        public string CurrentAddress { get; set; }

        public string CardNo { get; set; }

        public DateTime? IssuedDate { get; set; }

        public string IssuedPlace { get; set; }

        public string PassportNo { get; set; }

        public DateTime? PassportExp { get; set; }

        [DataType(DataType.Date)]
        public DateTime GroupDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JoinedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ConfirmedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ResignedDate { get; set; }

        public bool Enabled { get; set; }

        public bool IsSpecialUser { get; set; }

        public bool AutoApproveRequest { get; set; }

        public bool IsLocal { get; set; }

        public Toeic ToeicScore { get; set; }

        public string GeoMapLatitude { get; set; }

        public string GeoMapLongtitude { get; set; }

        public string DistanceToCompany { get; set; }

        public string ContactName { get; set; }

        public string UserRelationship { get; set; }

        public string ContactPhone { get; set; }

        public string ContactAddress { get; set; }

        public string LastUrl { get; set; }

        public string ConversationId { get; set; }

        public int JobGradeId { get; set; }

        public string JobGradeCode { get; set; }

        public string JobGradeName { get; set; }

        public DateTime? AffectedDate { get; set; }

        public string Note { get; set; }

        public int ManagementGradeId { get; set; }

        public string ManagementGradeCode { get; set; }

        public string ManagementGradeName { get; set; }

        public DateTime? ManagementAffectedDate { get; set; }

        public string ManagementNote { get; set; }

        public string SocialInsuranceNo { get; set; }

        public IList<Team> Teams { get; set; }

        public IEnumerable<JobTitle> JobTitles { get; set; }

        public short SalaryGroupId { get; set; }

        public short PayrollCompanyId { get; set; }

        public string[] RoleArray
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Roles))
                {
                    return new string[0];
                }

                return Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            set { Roles = Utilities.StringArrayToString(value); }
        }

        public string[] SubGroupArray
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SubGroups))
                {
                    return new string[0];
                }

                return SubGroups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            set { SubGroups = Utilities.StringArrayToString(value); }
        }

        private void InitData(Employee employee)
        {
            if (employee != null)
            {
                Id = employee.Id;
                Code = employee.Code;
                NickName = employee.NickName;
                FullName = employee.FullName;
                DomainUsername = employee.DomainUsername;
                EmployeeCard = employee.EmployeeCard;
                Roles = employee.Roles;
                TeamId = employee.Team.Id;
                JobTitleId = employee.JobTitle.Id;
                SubGroups = employee.SubGroups;
                DateOfBirth = employee.DateOfBirth;
                Email = employee.Email;
                Phone = employee.Phone;
                Gender = employee.Gender;
                Skype = employee.Skype;
                Yahoo = employee.Yahoo;
                Marriage = employee.Marriage;
                Address = employee.Address;
                PermanentAddress = employee.PermanentAddress;
                CurrentAddress = employee.CurrentAddress;
                CardNo = employee.CardNo;
                IssuedDate = employee.IssuedDate;
                IssuedPlace = employee.IssuedPlace;
                PassportNo = employee.PassportNo;
                PassportExp = employee.PassportExp;
                JoinedDate = employee.JoinedDate;
                ConfirmedDate = employee.ConfirmedDate;
                ResignedDate = employee.ResignedDate;
                Enabled = employee.Enabled;
                IsSpecialUser = employee.IsSpecialUser;
                AutoApproveRequest = employee.AutoApproveRequest;
                IsLocal = employee.IsLocal;
                ToeicScore = employee.ToeicScore;
                GeoMapLatitude = employee.GeoMapLatitude;
                GeoMapLongtitude = employee.GeoMapLongtitude;
                DistanceToCompany = employee.DistanceToCompany;
                PlaceOfBirth = employee.PlaceOfBirth;
                ContactName = employee.ContactName;
                UserRelationship = employee.UserRelationship;
                GroupDate = employee.GroupDate;
                ContactPhone = employee.ContactPhone;
                ContactAddress = employee.ContactAddress;
                EducationBackground = employee.EducationBackground;
                JobGradeId = employee.JobGradeId;
                JobGradeCode = employee.JobGradeCode;
                JobGradeName = employee.JobGradeName;
                AffectedDate = employee.AffectedDate;
                Note = employee.Note;
                ManagementGradeId = employee.ManagementGradeId;
                ManagementGradeCode = employee.ManagementGradeCode;
                ManagementGradeName = employee.ManagementGradeName;
                ManagementAffectedDate = employee.ManagementAffectedDate;
                ManagementNote = employee.ManagementNote;
                SocialInsuranceNo = employee.SocialInsuranceNo;
                SalaryGroupId = employee.SalaryGroupId;
                PayrollCompanyId = employee.PayrollCompanyId;
            }
        }
    }
}