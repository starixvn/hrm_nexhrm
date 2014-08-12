// --------------------------------------------------------------------------
// <copyright file="GetWorkingScheduleCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.10">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Working Schedule Criteria.
    /// </summary>
    public class GetWorkingScheduleCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime ReportDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Report_EmployeeCalendarByMonth";
        }
    }
}
