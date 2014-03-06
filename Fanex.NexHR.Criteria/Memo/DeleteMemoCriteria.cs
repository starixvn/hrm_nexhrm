// --------------------------------------------------------------------------
// <copyright file="DeleteMemoCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Memo
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Delete Memo Criteria.
    /// </summary>
    public class DeleteMemoCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Memo_Delete";
        }
    }
}
