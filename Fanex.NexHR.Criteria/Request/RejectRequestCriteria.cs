namespace Fanex.NexHR.Criteria.Request;

using System;

public class RejectRequestCriteria : BaseCommand
{
    public long RequestId { get; set; }

    public int ProcessedBy { get; set; }

    public DateTime ProcessedDate { get; set; }

    public override bool IsValid()
    {
        return true;
    }

    public override string GetSettingKey()
    {
        return "WorkingTimeRequest_RejectRequest";
    }
}