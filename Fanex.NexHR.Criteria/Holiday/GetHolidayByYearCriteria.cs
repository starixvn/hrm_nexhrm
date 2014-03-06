// -----------------------------------------------------------------------
// <copyright file="GetHolidayByYearCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Holiday
{
    using Fanex.NAP.Framework.Data;

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
