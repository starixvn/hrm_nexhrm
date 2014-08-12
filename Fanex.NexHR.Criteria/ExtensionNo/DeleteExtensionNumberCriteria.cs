// --------------------------------------------------------------------------
// <copyright file="DeleteExtensionNumberCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.3.14">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.ExtensionNo
{
    using Fanex.Data.Repository;

    /// <summary>
    /// DeleteExtensionNumberCriteria Criteria.
    /// </summary>
    public class DeleteExtensionNumberCriteria : CriteriaBase
    {
        public int ExtId { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_DELETE_EmployeeInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
