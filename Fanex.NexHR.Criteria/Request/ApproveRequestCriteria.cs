namespace Fanex.NexHR.Criteria.Request
{
    using System;

    public class ApproveRequestCriteria : BaseCommand
    {
        public long RequestId { get; set; }

        public int ApprovedBy { get; set; }

        public DateTime ApprovedDate { get; set; }

        public override bool IsValid()
        {
            return RequestId > 0 && ApprovedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_ApproveRequest";
        }
    }
}