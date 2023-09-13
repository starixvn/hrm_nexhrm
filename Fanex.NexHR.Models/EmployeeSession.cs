namespace Fanex.NexHR.Models
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    public class EmployeeSession : EntityBase<int>
    {
        public int UserId { get; set; }

        public bool Enabled { get; set; }

        public bool IsDeleted { get; set; }

        public string Roles { get; set; }

        public Team Team { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string DisplayName => NickName + (string.IsNullOrWhiteSpace(FullName) ? string.Empty : " (" + FullName + ")");

        public string CodeOfTeam => Team != null ? Team.Code : string.Empty;

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

            return roles.Any(role => RoleList.Contains((short)role));
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
