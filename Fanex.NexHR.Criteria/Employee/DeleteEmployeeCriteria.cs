// --------------------------------------------------------------------------
// <copyright file="DeleteEmployeeCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Delete Employee Criteria.
    /// </summary>
    public class DeleteEmployeeCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public DateTime ResignedDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_Delete";
        }
    }
}
