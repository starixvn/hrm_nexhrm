// --------------------------------------------------------------------------
// <copyright file="GetLeaderForStanbyTeamCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.14">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetLeaderForStanbyTeamCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetLeaderForStanbyTeam";
        }
    }
}
