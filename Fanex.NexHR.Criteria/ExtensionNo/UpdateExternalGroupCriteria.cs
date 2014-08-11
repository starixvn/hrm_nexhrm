// --------------------------------------------------------------------------
// <copyright file="UpdateExternalGroupCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// AddNewExternalGroup Criteria.
    /// </summary>
    public class UpdateExternalGroupCriteria : CriteriaBase
    {
        public int ExtId { get; set; }

        public string FullName { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_Update_GroupExternal";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
