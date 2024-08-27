namespace Fanex.NexHR.Criteria.Overtime
{
    using Data.Repository;

    public class ReportOvertimeHRDetailCriteria : CriteriaBase
    {
        public int Id{ get; set; }

        public override string GetSettingKey() => "Overtime_ReportHRDetail";

        public override bool IsValid() => Id > 0;
    }
}
