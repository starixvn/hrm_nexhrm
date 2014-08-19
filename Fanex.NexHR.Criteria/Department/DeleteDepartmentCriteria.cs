// -----------------------------------------------------------------------
// <copyright file="DeleteDepartmentCriteria.cs" company="Nexcel Solutions Viet Nam">
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
    /// Delete department criteria.
    /// </summary>
    public class DeleteDepartmentCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Delete";
        }

        public override bool IsValid()
        {
            return DepartmentId > 0;
        }
    }
}
