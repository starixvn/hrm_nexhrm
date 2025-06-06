namespace Fanex.NexHR.Criteria.ExtensionNo;

public class DeleteExternalGroupCriteria : BaseCommand
{
    public int ExtId { get; set; }

    public override string GetSettingKey()
    {
        return "ExtensionNO_DELETE_GroupExternal";
    }

    public override bool IsValid()
    {
        return true;
    }
}