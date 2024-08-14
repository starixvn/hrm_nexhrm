namespace Fanex.NexHR.Models
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    public class Employee : EntityBase<int>
    {
        public int UserId { get; set; }
        public string DomainUsername { get; set; }

        [Required]
        public string FullName { get; set; }

        public string NickName { get; set; }

        public string EmployeeCard { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }

        public bool Enabled { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsSpecialUser { get; set; }

        public string Phone { get; set; }

        public byte? Gender { get; set; }

        public string CodeOfTeam => Team != null ? Team.Code : string.Empty;

        public Team Team { get; set; }

        public string Roles { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JoinedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ConfirmedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ResignedDate { get; set; }

        public string Skype { get; set; }

        public string Yahoo { get; set; }

        public bool Marriage { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee Creator { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Employee Modifier { get; set; }

        [Display(Name = "Other team(s)")]
        public string SubGroups { get; set; }

        public int EmployeeRank { get; set; }

        public JobTitle JobTitle { get; set; }

        public string JobTitles => JobTitle != null ? JobTitle.Name : string.Empty;

        public string Address { get; set; }

        public string PermanentAddress { get; set; }

        public string CurrentAddress { get; set; }

        public string PlaceOfBirth { get; set; }

        public string ContactName { get; set; }

        public string UserRelationship { get; set; }

        public string ContactPhone { get; set; }

        public string ContactAddress { get; set; }

        public string CardNo { get; set; }

        public DateTime? IssuedDate { get; set; }

        public string IssuedPlace { get; set; }

        public string PassportNo { get; set; }

        public DateTime? PassportExp { get; set; }

        public string GeoMapLatitude { get; set; }

        public string GeoMapLongtitude { get; set; }

        public string DistanceToCompany { get; set; }

        public Toeic ToeicScore { get; set; }

        public int? Listening => ToeicScore != null ? ToeicScore.Listening : 0;

        public int? Reading => ToeicScore != null ? ToeicScore.Reading : 0;

        public int? LRTotal => ToeicScore != null ? ToeicScore.LRTotal : 0;

        public int? RequiredScore => ToeicScore != null ? ToeicScore.MinimumScore : 0;

        public DateTime? InputDate => ToeicScore != null ? ToeicScore.InputDate : null;

        public DateTime? LRExamDate => ToeicScore != null ? ToeicScore.LRExamDate : null;

        public string ConversationId { get; set; }

        public int JobGradeId { get; set; }

        public string JobGradeCode { get; set; }

        public string JobGradeName { get; set; }

        public DateTime AffectedDate { get; set; }

        public string Note { get; set; }

        public string SocialInsuranceNo { get; set; }

        public DefaultSchedule DefaultSchedule { get; set; }

        public DateTime GroupDate { get; set; }

        public string EducationBackground { get; set; }
        public IList<TrainingAndDevelopment> TrainingAndDevelopments { get; set; }
        public IList<TrainingAndDevelopmentType> TrainingAndDevelopmentTypes { get; set; }

        public IList<short> RoleList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Roles))
                {
                    return !HasRolePM ? new List<short>() : new List<short> { (short)Role.PM };
                }

                var roles = Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var result = new List<short>();

                foreach (var item in roles)
                {
                    result.Add(short.Parse(item));
                }

                if (HasRolePM)
                {
                    result.Add((short)Role.PM);
                }

                return result;
            }
        }

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

            set
            {
                Roles = Utilities.StringArrayToString(value);
            }
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

            set
            {
                SubGroups = Utilities.StringArrayToString(value);
            }
        }

        public string DisplayName => NickName + (string.IsNullOrWhiteSpace(FullName) ? string.Empty : " (" + FullName + ")");

        public bool HasRolePM => Team != null && Team.Code == "PM";

        public bool IsInRole(Role role)
        {
            return RoleList.Any(r => (short)role == r);
        }

        public bool IsInRoles(params Role[] roles)
        {
            if (RoleList == null)
            {
                return false;
            }

            return Array.Exists(roles, role => RoleList.Contains((short)role));
        }

        public static bool IsInRoles(IList<string> roles)
        {
            if (roles == null || roles.Count == 0)
            {
                return false;
            }

            return true;
        }

        public string DisplayRoles()
        {
            StringBuilder roles = new StringBuilder("Member");
            if (string.IsNullOrWhiteSpace(Roles))
            {
                return roles.ToString();
            }

            foreach (var item in RoleArray)
            {
                switch (item)
                {
                    case "1":
                        roles.Append(", AssistantLeader");
                        break;

                    case "2":
                        roles.Append(", Leader");
                        break;

                    case "3":
                        roles.Append(", HR");
                        break;

                    case "4":
                        roles = new StringBuilder("System admin");
                        break;

                    default:
                        roles = new StringBuilder("Member");
                        break;
                }
            }

            return roles.ToString();
        }
    }
}