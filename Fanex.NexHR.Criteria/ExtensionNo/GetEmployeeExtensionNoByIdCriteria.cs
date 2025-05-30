namespace Fanex.NexHR.Criteria.ExtensionNo;

using Data.Repository;

public class GetEmployeeExtensionNoByIdCriteria : CriteriaBase
{
    public bool IsInternal { get; set; }

    public int EmpExtId { get; set; }

    public override string GetSettingKey()
    {
        return "ExtensionNO_GetEmployeeInfo";
    }

    public override bool IsValid()
    {
        return true;
    }
}