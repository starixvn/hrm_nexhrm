// -----------------------------------------------------------------------
// <copyright file="GetDepartmentByIdCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.08.19">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Department
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get department by Id criteria.
    /// </summary>
    public class GetDepartmentByIdCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public override string GetSettingKey()
        {
            return "Department_GetById";
        }

        public override bool IsValid()
        {
            return DepartmentId > 0;
        }
    }
}
