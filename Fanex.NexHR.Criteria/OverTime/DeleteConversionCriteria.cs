namespace Fanex.NexHR.Criteria.Overtime
{
    public class DeleteConversionCriteria : BaseCommand
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_DeleteConversion";
        }

        public override bool IsValid()
        {
            return Id > 0 && ModifiedBy > 0;
        }
    }
}