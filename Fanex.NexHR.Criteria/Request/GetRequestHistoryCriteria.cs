// --------------------------------------------------------------------------
// <copyright file="GetRequestHistoryCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.7.31">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get request history criteria.
    /// </summary>
    public class GetRequestHistoryCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetRequestHistory";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
