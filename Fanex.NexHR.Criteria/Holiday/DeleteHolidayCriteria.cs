namespace Fanex.NexHR.Criteria.Holiday
{
    public class DeleteHolidayCriteria : BaseCommand
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_Delete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}