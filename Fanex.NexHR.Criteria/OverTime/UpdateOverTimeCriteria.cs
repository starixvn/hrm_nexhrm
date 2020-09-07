namespace Fanex.NexHR.Criteria.Overtime
{
    public class UpdateOvertimeCriteria : BaseActionOvertimeCriteria
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }
        
        public override string GetSettingKey()
        {
            return "Overtime_Update";
        }

        public override bool IsValid()
        {
            return base.IsValid() && Id > 0 && ModifiedBy > 0;
        }
    }
}
