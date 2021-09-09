namespace Fanex.NexHR.Criteria.EmployeeTransfer
{
    using Fanex.Data.Repository;

    public class GetLuminetShiftCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "EmployeeTransfer_GetLumiShift";
        
    }
}
