// --------------------------------------------------------------------------
// <copyright file="DeleteRequestCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Delete Request Criteria.
    /// </summary>
    public class DeleteRequestCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Delete";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
