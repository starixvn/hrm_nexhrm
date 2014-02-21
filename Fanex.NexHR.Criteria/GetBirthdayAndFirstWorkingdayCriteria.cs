// --------------------------------------------------------------------------
// <copyright file="GetBirthdayAndFirstWorkingdayCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.2.17">Created</change>
// </history>

namespace Fanex.NexHR.Criteria
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get Birthday And First Working day Criteria.
    /// </summary>
    public class GetBirthdayAndFirstWorkingdayCriteria : CriteriaBase
    {
        public DateTime OnDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetBirthdayAndFirstWorkingday";
        }
    }
}
