namespace Fanex.NexHR.Criteria.Holiday
{
    using System;

    public class UpdateHolidayCriteria : BaseCommand
    {
        public int Id { get; set; }

        public string HolidayName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_Update";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate && Id > 0 && !string.IsNullOrWhiteSpace(HolidayName);
        }
    }
}