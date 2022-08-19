// --------------------------------------------------------------------------
// <copyright file="DisapproveRequestCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.11">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;

    /// <summary>
    /// Disapprove Request Criteria.
    /// </summary>
    public class DisapproveRequestCriteria : BaseCommand
    {
        public long RequestId { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_DisapproveRequest";
        }
    }
}