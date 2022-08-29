namespace Fanex.NexHR.Criteria.WorkFromHomeRequest
{
    public class DeleteWorkFromHomeRequestCriteria : BaseCommand
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