// -----------------------------------------------------------------------
// <copyright file="GetAnnualLeaveByIdCriteria.cs" company="Nexcel Solutions Viet Nam">
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
    /// Get annual leave by id criteria.
    /// </summary>
    public class GetAnnualLeaveByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_GetByID";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
