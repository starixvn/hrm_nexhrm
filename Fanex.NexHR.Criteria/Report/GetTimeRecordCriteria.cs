// --------------------------------------------------------------------------
// <copyright file="GetTimeRecordCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.7.14">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;

    public class GetTimeRecordCriteria : CriteriaBase
    {
        public int? EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string GroupIds { get; set; }

        public int? PageIndex { get; set; }

        public int? ItemsPerPage { get; set; }

        [Output]
        public int? TotalItem { get; set; }

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