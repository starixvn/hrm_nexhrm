// --------------------------------------------------------------------------
// <copyright file="InsertEmployeeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.7">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Insert Employee Criteria.
    /// </summary>
    public class InsertEmployeeCriteria : CriteriaBase
    {
        public string FullName { get; set; }

        public string NickName { get; set; }

        public string DomainUsername { get; set; }

        public string Code { get; set; }

        public int? MonShift { get; set; }

        public int? TueShift { get; set; }

        public int? WedShift { get; set; }

        public int? ThuShift { get; set; }

        public int? FriShift { get; set; }

        public int? SatShift { get; set; }

        public int? SunShift { get; set; }

        public int MainGroupId { get; set; }

        public DateTime Createddate { get; set; }

        public int CreateBy { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_Insert";
        }
    }
}
