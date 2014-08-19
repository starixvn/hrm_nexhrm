// -----------------------------------------------------------------------
// <copyright file="UpdateDepartmentCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.08.19">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Department
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Update department criteria.
    /// </summary>
    public class UpdateDepartmentCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public int? ManagerId { get; set; }

        public bool? IsDisabled { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Update";
        }

        public override bool IsValid()
        {
            return DepartmentId > 0;
        }
    }
}
