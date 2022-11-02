// --------------------------------------------------------------------------
// <copyright file="TimeRecordUpdateTypeCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.7.14">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using System;

    /// <summary>
    /// TimeRecordUpdateType Criteria.
    /// </summary>
    public class TimeRecordUpdateTypeCriteria : BaseCommand
    {
        public int RecordTimeId { get; set; }
        public int EmployeeId { get; set; }

        public short? InOutType { get; set; }

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