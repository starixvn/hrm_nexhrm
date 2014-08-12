// --------------------------------------------------------------------------
// <copyright file="GetTeamByIdCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get Team By Id Criteria.
    /// </summary>
    public class GetTeamByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "GetGroup";
        }
    }
}
