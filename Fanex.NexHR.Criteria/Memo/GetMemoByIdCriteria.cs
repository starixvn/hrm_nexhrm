// --------------------------------------------------------------------------
// <copyright file="GetMemoByIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Memo
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Memo By Id.
    /// </summary>
    public class GetMemoByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Memo_GetById";
        }
    }
}
