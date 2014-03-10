// -----------------------------------------------------------------------
// <copyright file="GetHalfDayOffCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get half day off criteria.
    /// </summary>
    public class GetHalfDayOffCriteria : CriteriaBase
    {
        public DateTime ReportDate { get; set; }

        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetHalfDayOff";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
