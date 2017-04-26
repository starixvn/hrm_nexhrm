namespace Fanex.NexHR.Criteria.Holiday
{
    using Data.Repository;

    public class DeleteHolidayCriteria : CriteriaBase
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
