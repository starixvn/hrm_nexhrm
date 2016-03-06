namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;

    public class DeleteRequestCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Delete";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
