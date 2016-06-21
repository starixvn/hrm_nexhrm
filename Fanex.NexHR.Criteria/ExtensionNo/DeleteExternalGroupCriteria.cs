// --------------------------------------------------------------------------
// <copyright file="DeleteExternalGroupCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.3.14">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.ExtensionNo
{
    using Fanex.Data.Repository;

    /// <summary>
    /// DeleteExternalGroup Criteria.
    /// </summary>
    public class DeleteExternalGroupCriteria : CriteriaBase
    {
        public int ExtId { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_DELETE_GroupExternal";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
