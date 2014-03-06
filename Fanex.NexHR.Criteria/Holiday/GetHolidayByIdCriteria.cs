// -----------------------------------------------------------------------
// <copyright file="GetHolidayByIdCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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
