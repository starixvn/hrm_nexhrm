// --------------------------------------------------------------------------
// <copyright file="GetEmployeeShiftCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.7">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Employee Shift.
    /// </summary>
    public class GetEmployeeShiftCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            if (Id > 0)
            {
                return true;
            }

            return false;
        }

        public override string GetSettingKey()
        {
            return "EmployeeShift_Get";
        }
    }
}
