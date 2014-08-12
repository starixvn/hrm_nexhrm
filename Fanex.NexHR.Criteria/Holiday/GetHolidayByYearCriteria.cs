// -----------------------------------------------------------------------
// <copyright file="GetHolidayByYearCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Holiday
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get holiday by year criteria.
    /// </summary>
    public class GetHolidayByYearCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_GetByYear";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
