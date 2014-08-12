// -----------------------------------------------------------------------
// <copyright file="GetUserChangedShiftCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get user changed shift criteria.
    /// </summary>
    public class GetUserChangedShiftCriteria : CriteriaBase
    {
        public DateTime ReportDate { get; set; }

        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetChangedShift";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
