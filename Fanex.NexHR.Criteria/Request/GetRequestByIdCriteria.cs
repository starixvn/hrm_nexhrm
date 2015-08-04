// --------------------------------------------------------------------------
// <copyright file="GetRequestByIdCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get Request By Id Criteria.
    /// </summary>
    public class GetRequestByIdCriteria : CriteriaBase
    {
        public long Id { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetById";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
