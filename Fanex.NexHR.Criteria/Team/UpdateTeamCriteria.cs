// --------------------------------------------------------------------------
// <copyright file="UpdateTeamCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Update Team Criteria.
    /// </summary>
    public class UpdateTeamCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int LeaderId { get; set; }

        public int? AssistantLeaderId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Update";
        }
    }
}
