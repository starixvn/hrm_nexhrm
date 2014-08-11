// --------------------------------------------------------------------------
// <copyright file="DisapproveRequestCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Disapprove Request Criteria.
    /// </summary>
    public class DisapproveRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

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
