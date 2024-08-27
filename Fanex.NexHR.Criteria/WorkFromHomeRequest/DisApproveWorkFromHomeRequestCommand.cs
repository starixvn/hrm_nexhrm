namespace Fanex.NexHR.Criteria.WorkFromHomeRequest
{
    public class DisApproveWorkFromHomeRequestCommand : BaseCommand
    {
        public int WorkFromHomeRequestId { get; set; }

        public int ProcessedBy { get; set; }

        public override string GetSettingKey() => "WorkFromHomeRequest_DisApproveRequest";

        public override bool IsValid() => WorkFromHomeRequestId > 0 && ProcessedBy > 0;
    }
}