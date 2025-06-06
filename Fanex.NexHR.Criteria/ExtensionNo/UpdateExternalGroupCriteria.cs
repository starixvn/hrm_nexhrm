namespace Fanex.NexHR.Criteria.ExtensionNo;

public class UpdateExternalGroupCriteria : BaseCommand
{
    public int ExtId { get; set; }

    public string FullName { get; set; }

    public override string GetSettingKey()
    {
        return "ExtensionNO_Update_GroupExternal";
    }

    public override bool IsValid()
    {
        return true;
    }
}