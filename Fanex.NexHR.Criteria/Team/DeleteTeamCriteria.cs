// --------------------------------------------------------------------------
// <copyright file="DeleteTeamCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Team
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Delete Team Criteria.
    /// </summary>
    public class DeleteTeamCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Delete";
        }
    }
}
