// --------------------------------------------------------------------------
// <copyright file="GetShiftByDateCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Shift
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Shift By Date Criteria.
    /// </summary>
    public class GetShiftByDateCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetShiftByDate";
        }
    }
}
