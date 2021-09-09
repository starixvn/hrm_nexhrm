namespace Fanex.NexHR.Criteria.EmployeeTransfer
{
    using Fanex.Data.Repository;

    public class GetLuminetGroupCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeTransfer_GetLumiGroup";
        }
    }
}
