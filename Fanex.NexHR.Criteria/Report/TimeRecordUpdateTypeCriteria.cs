// --------------------------------------------------------------------------
// <copyright file="TimeRecordUpdateTypeCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// TimeRecordUpdateType Criteria.
    /// </summary>
    public class TimeRecordUpdateTypeCriteria : CriteriaBase
    {
        public int RecordTimeId { get; set; }

        public byte InOutType { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "TimeRecord_UpdateType";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
