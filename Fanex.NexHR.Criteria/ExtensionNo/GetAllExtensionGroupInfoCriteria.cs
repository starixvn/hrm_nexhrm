// --------------------------------------------------------------------------
// <copyright file="GetAllExtensionGroupInfoCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.3.14">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.ExtensionNo
{
    using Data.Repository;

    /// <summary>
    /// GetAllExtensionGroupInfo Criteria.
    /// </summary>
    public class GetAllExtensionGroupInfoCriteria : CriteriaBase
    {
        public bool IsInternal { get; set; }

        public int? GroupId { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_GetGroupInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
