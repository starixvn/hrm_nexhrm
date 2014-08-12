// -----------------------------------------------------------------------
// <copyright file="GetMemberByManagerCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get member by manager criteria.
    /// </summary>
    public class GetMemberByManagerCriteria : CriteriaBase
    {
        public int ManagerId { get; set; }

        public int Role { get; set; }

        public bool ExcludedManager { get; set; }

        public int? GroupId { get; set; }

        public string Keyword { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetMembesByManager";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
