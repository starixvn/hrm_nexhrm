// --------------------------------------------------------------------------
// <copyright file="UpdateExtensionNoMemberCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// UpdateExtensionNoMember Criteria.
    /// </summary>
    public class UpdateExtensionNoMemberCriteria : CriteriaBase
    {
        public int ExtId { get; set; }

        public int GroupId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string ExtensionNo { get; set; }

        public bool IsInternal { get; set; }

        public int EmpId { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_Update_EmployeeInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
