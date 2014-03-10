// -----------------------------------------------------------------------
// <copyright file="UpdateHolidayCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Holiday
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Update holiday criteria.
    /// </summary>
    public class UpdateHolidayCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string HolidayName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_Update";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
