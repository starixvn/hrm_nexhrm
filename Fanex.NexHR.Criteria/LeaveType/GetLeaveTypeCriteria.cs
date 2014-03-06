// -----------------------------------------------------------------------
// <copyright file="GetLeaveTypeCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.LeaveType
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get leave type criteria.
    /// </summary>
    public class GetLeaveTypeCriteria : CriteriaBase
    {
        public bool ExcludeNotShown
        {
            get
            {
                return true;
            }

            set 
            {
                ExcludeNotShown = value;
            }
        }

        public override string GetSettingKey()
        {
            return "LeaveType_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
