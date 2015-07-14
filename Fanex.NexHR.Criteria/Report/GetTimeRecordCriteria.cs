// --------------------------------------------------------------------------
// <copyright file="GetTimeRecordCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.7.14">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Time Record Criteria.
    /// </summary>
    public class GetTimeRecordCriteria : CriteriaBase
    {
        public int? EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string GroupIds { get; set; }

        public override string GetSettingKey()
        {
            return "TimeRecord_Get";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
