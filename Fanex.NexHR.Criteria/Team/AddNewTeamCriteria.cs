// --------------------------------------------------------------------------
// <copyright file="AddNewTeamCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Team
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Add New Team Criteria.
    /// </summary>
    public class AddNewTeamCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int LeaderId { get; set; }

        public int AssistantLeaderId { get; set; }

        public int? DepartmentId { get; set; }

        public int? ParentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Insert";
        }
    }
}
