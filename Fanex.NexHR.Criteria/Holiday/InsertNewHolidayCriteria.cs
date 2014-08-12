// -----------------------------------------------------------------------
// <copyright file="InsertNewHolidayCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Holiday
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Insert holiday criteria.
    /// </summary>
    public class InsertNewHolidayCriteria : CriteriaBase
    {
        public string HolidayName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_Insert";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
