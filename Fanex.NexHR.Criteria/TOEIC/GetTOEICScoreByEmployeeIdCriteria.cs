// -----------------------------------------------------------------------
// <copyright file="GetTOEICScoreByEmployeeIdCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.TOEIC
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get TOEIC Score By Employee Id.
    /// </summary>
    public class GetTOEICScoreByEmployeeIdCriteria : CriteriaBase
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
