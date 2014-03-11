// -----------------------------------------------------------------------
// <copyright file="GetHolidayByIdCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Holiday
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get holiday by ID criteria.
    /// </summary>
    public class GetHolidayByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_GetById";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
