namespace Fanex.NexHR.Criteria.AnnualLeave;

using Data.Repository;

public class GetAnnualLeaveTransactionByMonthCriteria : CriteriaBase
{
    private const byte December = 12;

    public int EmpId { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public override string GetSettingKey() => "AnnualLeave_GetEmpTransByMonth";

    public override bool IsValid() => EmpId > 0 && Year > 0 && Month is > 0 and <= December;
}