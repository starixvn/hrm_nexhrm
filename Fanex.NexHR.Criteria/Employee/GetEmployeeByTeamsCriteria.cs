// --------------------------------------------------------------------------
// <copyright file="GetEmployeeByTeamsCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeByTeamsCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string TeamCodes { get; set; }

        public string IgnoreTeams { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetEmployeeByTeams";
        }
    }
}
