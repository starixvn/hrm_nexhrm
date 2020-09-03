namespace Fanex.NexHR.Criteria.Overtime
{
    using Data.Repository;

    public class ResetOvertimeCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_ResetRequest";
        }

        public override bool IsValid()
        {
            return Id > 0 && ModifiedBy > 0;
        }
    }
}
