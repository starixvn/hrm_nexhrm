// --------------------------------------------------------------------------
// <copyright file="GetEmployeeByTeamsCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.11">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get Employee By Teams Criteria.
    /// </summary>
    public class GetEmployeeByTeamsCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string TeamCodes { get; set; }

        public string IgnoreTeams { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetEmployeeByTeams";
        }
    }
}
