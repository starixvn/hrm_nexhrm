namespace Fanex.NexHR.Criteria.Overtime
{
    using Data.Repository;
    using System;

    public class GetNexHourCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public float Hour { get; set; }

        public DateTime Date { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GetNexHour";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0 && Hour > 0;
        }
    }
}
