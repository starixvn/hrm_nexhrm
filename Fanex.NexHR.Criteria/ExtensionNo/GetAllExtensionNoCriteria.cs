// --------------------------------------------------------------------------
// <copyright file="GetAllExtensionNoCriteria.cs" company=".">
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
    /// GetAllExtensionNo Criteria.
    /// </summary>
    public class GetAllExtensionNoCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "ExtensionNO_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
