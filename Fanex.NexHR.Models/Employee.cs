namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using Attributes;
    using Core;
    using Core.Enums;

    public class Employee : EntityBase<int>
    {
        public string DomainUsername { get; set; }

        [Required]
        public string FullName { get; set; }

        public string NickName { get; set; }

        public string EmployeeCard { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Email]
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

        public bool Mariage { get; set; }

        public DateTime? LastLogin { get; set; }

        public decimal AnnualLeaveBalance { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee Creator { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Employee Modifier { get; set; }

        [Display(Name = "Other team(s)")]
        public string SubGroups { get; set; }

        [Display(Name = "Teams' leaders receive requests")]
        public string LeaderGroups { get; set; }

        public JobTitle JobTitle { get; set; }

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

        public string ConversationId { get; set; }

        public DefaultSchedule DefaultSchedule { get; set; }

        public IList<short> RoleList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Roles))
                {
                    return null;
                }

                var roles = Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var result = new List<short>();

                foreach (var item in roles)
                {
                    result.Add(short.Parse(item));
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
                    return null;
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
                    return null;
                }

                return SubGroups.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            set
            {
                SubGroups = Utilities.StringArrayToString(value);
            }
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

            set
            {
                LeaderGroups = Utilities.StringArrayToString(value);
            }
        }

        public string DisplayName => NickName + (string.IsNullOrWhiteSpace(FullName) ? string.Empty : " (" + FullName + ")");

        public bool HasRolePM => Team != null && Team.Code == "PM";

        public bool IsInRoles(params Role[] roles)
        {
            if (RoleList == null)
            {
                return false;
            }

            var userRoles = System.Web.Security.Roles.GetRolesForUser();
            return roles.Any(role => userRoles.Contains(role.EnumToString()));
        }

        public static bool IsInRoles(IList<string> roles)
        {
            if (roles == null || roles.Count == 0)
            {
                return false;
            }

            var userRoles = System.Web.Security.Roles.GetRolesForUser();
            return roles.Any(role => userRoles.Contains(role));
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