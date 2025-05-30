namespace Fanex.NexHR.Criteria.Request;

using System;

public class DisapproveRequestCriteria : BaseCommand
{
    public long RequestId { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public override bool IsValid()
    {
        return true;
    }

    public override string GetSettingKey()
    {
        return "WorkingTimeRequest_DisapproveRequest";
    }
}