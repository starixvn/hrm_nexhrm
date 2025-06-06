namespace Fanex.NexHR.Criteria.Request;

using Data.Repository;

public class GetRequestByIdCriteria : CriteriaBase
{
    public long Id { get; set; }

    public override string GetSettingKey()
    {
        return "WorkingTimeRequest_GetById";
    }

    public override bool IsValid()
    {
        return Id > 0;
    }
}