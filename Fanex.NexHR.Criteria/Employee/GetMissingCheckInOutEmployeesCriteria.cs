// -----------------------------------------------------------------------
// <copyright file="GetMissingCheckInOutEmployeesCriteria.cs" company="JNexel SolutionsViet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get missing check in out employees criteria.
    /// </summary>
    public class GetMissingCheckInOutEmployeesCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "GetMissingCheckInOutList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
