// -----------------------------------------------------------------------
// <copyright file="GetMissingCheckInOutEmployeesCriteria.cs" company="Nexel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get missing check in out employees criteria.
    /// </summary>
    public class GetMissingCheckInOutEmployeesCriteria : CriteriaBase
    {
        public int? EmpId { get; set; }
        public override string GetSettingKey()
        {
            return "GetMissingCheckInOutList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
