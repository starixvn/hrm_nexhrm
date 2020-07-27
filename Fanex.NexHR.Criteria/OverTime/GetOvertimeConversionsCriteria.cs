namespace Fanex.NexHR.Criteria.Overtime
{
    using Data.Repository;
    using System;

    public class GetOvertimeConversionsCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GetConversions_138194";
        }

        public override bool IsValid()
        {
            return DateTime.Compare(FromDate, ToDate) <= 0;
        }
    }
}
