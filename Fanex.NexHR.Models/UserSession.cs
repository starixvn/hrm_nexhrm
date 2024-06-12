namespace Fanex.NexHR.Models
{
    using Fanex.NexHR.Core.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserSession : EntityBase<int>
    {
        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public bool Enabled { get; set; }

        public bool IsDeleted { get; set; }

        public string Roles { get; set; }

        public int TeamId { get; set; }

        public string TeamCode { get; set; }

         public bool IsLeaderHR { get; set; }

        public string DisplayName => NickName + (string.IsNullOrWhiteSpace(FullName) ? string.Empty : " (" + FullName + ")");

        public string CodeOfTeam => TeamCode ?? string.Empty;

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

        public bool HasRolePM => TeamCode == "PM";

        public bool HasHRRole => RoleList.Any(r => r == (short)Role.HR || r == (short)Role.SA);

        public bool IsInRoles(params Role[] roles)
        {
            if (RoleList == null)
            {
                return false;
            }

            return Array.Exists(roles, role => RoleList.Contains((short)role));
        }
    }
}