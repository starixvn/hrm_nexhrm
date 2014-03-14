// --------------------------------------------------------------------------
// <copyright file="DeleteExternalGroupCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.3.14">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.ExtensionNo
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// DeleteExternalGroup Criteria.
    /// </summary>
    public class DeleteExternalGroupCriteria : CriteriaBase
    {
        public int ExtID { get; set; }

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
