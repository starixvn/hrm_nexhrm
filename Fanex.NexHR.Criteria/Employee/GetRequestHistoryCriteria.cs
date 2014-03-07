// -----------------------------------------------------------------------
// <copyright file="GetRequestHistoryCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get request history criteria.
    /// </summary>
    public class GetRequestHistoryCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int Type { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetRequestHistory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
