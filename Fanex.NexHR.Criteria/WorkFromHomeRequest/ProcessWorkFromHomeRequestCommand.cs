namespace Fanex.NexHR.Criteria.WorkFromHomeRequest
{
    public class ProcessWorkFromHomeRequestCommand : BaseCommand
    {
        public int WorkFromHomeRequestId { get; set; }

        public byte StatusId { get; set; }

        public int ProcessedBy { get; set; }

        public override string GetSettingKey() => "WorkFromHomeRequest_ProcessRequest";

        public override bool IsValid() => WorkFromHomeRequestId > 0 && StatusId > 0 && ProcessedBy > 0;
    }
}