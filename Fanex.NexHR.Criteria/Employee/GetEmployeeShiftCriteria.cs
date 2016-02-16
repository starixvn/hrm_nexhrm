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

    public class GetEmployeeShiftCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "EmployeeShift_Get";
        }
    }
}
