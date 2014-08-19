// -----------------------------------------------------------------------
// <copyright file="InsertDepartmentCriteria.cs" company="Nexcel Solutions Viet Nam">
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
    /// Insert new department criteria.
    /// </summary>
    public class InsertDepartmentCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public int? ManagerId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
