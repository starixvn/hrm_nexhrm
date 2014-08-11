// --------------------------------------------------------------------------
// <copyright file="ApproveRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.11">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Approve Request Criteria.
    /// </summary>
    public class ApproveRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int ApprovedBy { get; set; }

        public DateTime ApprovedDate { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_ApproveRequest";
        }
    }
}
