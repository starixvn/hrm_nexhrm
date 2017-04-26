namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetMissingCheckInOutEmployeesCriteria : CriteriaBase
    {
        public int? EmpId { get; set; }

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
