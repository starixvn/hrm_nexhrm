// --------------------------------------------------------------------------
// <copyright file="GetToeicScoreByEmployeeIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.5">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Toeic
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get TOEIC Score By Employee Id.
    /// </summary>
    public class GetToeicScoreByEmployeeIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "TOEIC_GetScore";
        }
    }
}
