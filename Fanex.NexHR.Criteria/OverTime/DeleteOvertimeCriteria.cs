namespace Fanex.NexHR.Criteria.Overtime
{
    using Data.Repository;

    public class DeleteOvertimeCriteria: CriteriaBase
    {
        public int Id { get; set; }

        public int ModifiedId { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_Delete";
        }

        public override bool IsValid()
        {
            return Id > 0 && ModifiedId > 0;
        }
    }
}
