// --------------------------------------------------------------------------
// <copyright file="OpenAnnualBalanceForNewYearCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.12.30">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// OpenAnnualBalanceForNewYear Criteria.
    /// </summary>
    public class OpenAnnualBalanceForNewYearCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public int? EmpId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Description { get; set; }

        public override string GetSettingKey()
        {
            return "DBA_AnnualLeave_CreateAnnualBalance";
        }

        public override bool IsValid()
        {
            return Year > 2010;
        }
    }
}
