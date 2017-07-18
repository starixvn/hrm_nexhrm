namespace Fanex.NexHR.ViewModels.Employee
{
    using Core;
    using Models;
    using Models.Attributes;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EditEmployeeViewModel
    {
        public EditEmployeeViewModel(Employee employee = null)
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

        public int JobTitleId { get; set; }

        public decimal AnnualLeaveBalance { get; set; }

        [Display(Name = "Other team(s)")]
        public string SubGroups { get; set; }

        [Display(Name = "Teams' leaders receive requests")]
        public string LeaderGroups { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Email]
        public string Email { get; set; }

        public string Phone { get; set; }

        public byte? Gender { get; set; }

        public string Skype { get; set; }

        public string Yahoo { get; set; }

        public bool Mariage { get; set; }

        public string Address { get; set; }

        public string PermanentAddress { get; set; }

        public string CurrentAddress { get; set; }

        public string CardNo { get; set; }

        public DateTime? IssuedDate { get; set; }

        public string IssuedPlace { get; set; }

        public string PassportNo { get; set; }

        public DateTime? PassportExp { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JoinedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ConfirmedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ResignedDate { get; set; }

        public bool Enabled { get; set; }

        public Toeic ToeicScore { get; set; }

        public string GeoMapLatitude { get; set; }

        public string GeoMapLongtitude { get; set; }

        public string DistanceToCompany { get; set; }

        public IList<Team> Teams { get; set; }

        public IList<JobTitle> JobTitles { get; set; }

        public string[] RoleArray
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Roles))
                {
                    return null;
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
                    return null;
                }

                return SubGroups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            set { SubGroups = Utilities.StringArrayToString(value); }
        }

        public string[] LeaderGroupsArray
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LeaderGroups))
                {
                    return null;
                }

                return LeaderGroups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            set { LeaderGroups = Utilities.StringArrayToString(value); }
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
                AnnualLeaveBalance = employee.AnnualLeaveBalance;
                SubGroups = employee.SubGroups;
                LeaderGroups = employee.LeaderGroups;
                DateOfBirth = employee.DateOfBirth;
                Email = employee.Email;
                Phone = employee.Phone;
                Gender = employee.Gender;
                Skype = employee.Skype;
                Yahoo = employee.Yahoo;
                Mariage = employee.Mariage;
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
                ToeicScore = employee.ToeicScore;
                GeoMapLatitude = employee.GeoMapLatitude;
                GeoMapLongtitude = employee.GeoMapLongtitude;
                DistanceToCompany = employee.DistanceToCompany;
            }
        }
    }
}