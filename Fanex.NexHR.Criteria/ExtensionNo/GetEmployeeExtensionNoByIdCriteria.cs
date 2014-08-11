// --------------------------------------------------------------------------
// <copyright file="GetEmployeeExtensionNoByIdCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// GetEmployeeExtensionNoById Criteria.
    /// </summary>
    public class GetEmployeeExtensionNoByIdCriteria : CriteriaBase
    {
        public bool IsInternal { get; set; }

        public int EmpExtId { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_GetEmployeeInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
