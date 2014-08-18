// -----------------------------------------------------------------------
// <copyright file="GetAllDepartmentCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.08.18">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Department
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetAllDepartmentCriteria : CriteriaBase
    {
        public int? ParentId { get; set; }

        public bool? IsDisabled { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
