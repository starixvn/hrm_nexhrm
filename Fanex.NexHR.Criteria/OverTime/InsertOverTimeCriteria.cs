namespace Fanex.NexHR.Criteria.Overtime
{
    public class InsertOvertimeCriteria : BaseActionOvertimeCriteria
    {
        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_Insert";
        }

        public override bool IsValid()
        {
            return base.IsValid() && CreatedBy > 0;
        }
    }
}
