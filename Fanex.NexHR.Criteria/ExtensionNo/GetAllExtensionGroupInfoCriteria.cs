namespace Fanex.NexHR.Criteria.ExtensionNo;

using Data.Repository;

public class GetAllExtensionGroupInfoCriteria : CriteriaBase
{
    public bool IsInternal { get; set; }

    public int? GroupId { get; set; }

    public override string GetSettingKey()
    {
        return "ExtensionNO_GetGroupInfo";
    }

    public override bool IsValid()
    {
        return true;
    }
}