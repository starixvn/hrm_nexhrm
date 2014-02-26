// -----------------------------------------------------------------------
// <copyright file="GetTeamHandleRequestOfEmployee.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get Team Handle Request Of Employee.
    /// </summary>
    public class GetTeamHandleRequestOfEmployee : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetGroupHandleRequest";
        }
    }
}
