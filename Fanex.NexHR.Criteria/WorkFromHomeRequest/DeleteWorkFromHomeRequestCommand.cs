namespace Fanex.NexHR.Criteria.WorkFromHomeRequest
{
    public class DeleteWorkFromHomeRequestCommand : BaseCommand
    {
        public int WorkFromHomeRequestId { get; set; }

        public int DeletedBy { get; set; }

        public override string GetSettingKey() => "WorkFromHomeRequest_Delete";

        public override bool IsValid() => WorkFromHomeRequestId > 0;
    }
}