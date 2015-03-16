// --------------------------------------------------------------------------
// <copyright file="GetOpenAndCloseAnnualBalanceCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.12.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetOpenAndCloseAnnualBalance Criteria.
    /// </summary>
    public class GetOpenAndCloseAnnualBalanceCriteria : CriteriaBase
    {
        public DateTime OpenDate { get; set; }

        public DateTime CloseDate { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_GetOpenAndCloseBalance";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
