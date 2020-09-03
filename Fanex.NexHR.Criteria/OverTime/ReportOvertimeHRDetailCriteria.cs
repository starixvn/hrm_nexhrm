namespace Fanex.NexHR.Criteria.Overtime
{
    using Data.Repository;

    public class ReportOvertimeHRDetailCriteria : CriteriaBase
    {
        public int Id{ get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_ReportHRDetail";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
