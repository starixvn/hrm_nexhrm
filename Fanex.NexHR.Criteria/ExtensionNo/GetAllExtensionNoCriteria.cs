namespace Fanex.NexHR.Criteria.ExtensionNo;

using Data.Repository;

public class GetAllExtensionNoCriteria : CriteriaBase
{
    public override string GetSettingKey()
    {
        return "ExtensionNO_GetAll";
    }

    public override bool IsValid()
    {
        return true;
    }
}