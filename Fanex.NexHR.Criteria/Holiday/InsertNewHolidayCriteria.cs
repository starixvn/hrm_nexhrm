namespace Fanex.NexHR.Criteria.Holiday
{
    using System;
    using Data.Repository;

    public class InsertNewHolidayCriteria : CriteriaBase
    {
        public string HolidayName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_Insert";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate && !string.IsNullOrWhiteSpace(HolidayName);
        }
    }
}
