// --------------------------------------------------------------------------
// <copyright file="GetTeamByManagerIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Team By Manager Id Criteria.
    /// </summary>
    public class GetTeamByManagerIdCriteria : CriteriaBase
    {
        public int ManagerId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "GetTeamManagedById";
        }
    }
}
