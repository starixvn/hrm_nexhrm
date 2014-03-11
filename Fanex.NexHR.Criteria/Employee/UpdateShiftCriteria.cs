// --------------------------------------------------------------------------
// <copyright file="UpdateShiftCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.10">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Update Shift Criteria.
    /// </summary>
    public class UpdateShiftCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime Date { get; set; }

        public int? ShiftID { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_Update";
        }
    }
}
