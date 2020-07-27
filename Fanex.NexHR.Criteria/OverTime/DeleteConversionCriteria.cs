namespace Fanex.NexHR.Criteria.Overtime
{
    using Data.Repository;

    public class DeleteConversionCriteria : CriteriaBase
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
