namespace Fanex.NexHR.Criteria.Overtime
{
    using Data.Repository;

    public class DisapproveOvertimeRequestCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public int Status { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_DisapproveRequest";
        }

        public override bool IsValid()
        {
            return Id > 0 && Status > 0;
        }
    }
}
