namespace Fanex.NexHR.Criteria.WorkFromHomeRequest
{
    using Fanex.Data.Repository;

    public class DeleteWorkFromHomeRequestCriteria : CriteriaBase
    {
        public int WorkFromHomeRequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkFromHomeRequest_Delete";
        }

        public override bool IsValid()
        {
            return WorkFromHomeRequestId > 0;
        }
    }
}
