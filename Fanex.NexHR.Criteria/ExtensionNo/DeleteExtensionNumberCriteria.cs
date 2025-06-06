namespace Fanex.NexHR.Criteria.ExtensionNo;

public class DeleteExtensionNumberCriteria : BaseCommand
{
    public int ExtId { get; set; }

    public override string GetSettingKey()
    {
        return "ExtensionNO_DELETE_EmployeeInfo";
    }

    public override bool IsValid()
    {
        return true;
    }
}