namespace Fanex.NexHR.Criteria.ExtensionNo;

public class UpdateExtensionNoMemberCriteria : BaseCommand
{
    public int ExtId { get; set; }

    public int GroupId { get; set; }

    public string FullName { get; set; }

    public string NickName { get; set; }

    public string ExtensionNo { get; set; }

    public bool IsInternal { get; set; }

    public int EmpId { get; set; }

    public override string GetSettingKey()
    {
        return "ExtensionNO_Update_EmployeeInfo";
    }

    public override bool IsValid()
    {
        return true;
    }
}