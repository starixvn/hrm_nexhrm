// -----------------------------------------------------------------------
// <copyright file="DeleteAnnualLeaveTransactionCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Delete annual leave transaction criteria.
    /// </summary>
    public class DeleteAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_DeleteTransaction";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
