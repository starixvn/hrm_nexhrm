// --------------------------------------------------------------------------
// <copyright file="RejectRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.11">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Reject Request Criteria.
    /// </summary>
    public class RejectRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int ProcessedBy { get; set; }

        public DateTime ProcessedDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_RejectRequest";
        }
    }
}
