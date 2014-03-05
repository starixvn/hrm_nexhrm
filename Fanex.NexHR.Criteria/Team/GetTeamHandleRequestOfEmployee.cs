// --------------------------------------------------------------------------
// <copyright file="GetTeamHandleRequestOfEmployee.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get Team Handle Request Of Employee.
    /// </summary>
    public class GetTeamHandleRequestOfEmployee : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetGroupHandleRequest";
        }
    }
}
