namespace Fanex.NexHR.Criteria.CompanyEvent
{
    using System;

    public class InsertEventCommand : BaseCommand
    {
        public string Title { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Title) && ToDate >= FromDate;

        public override string GetSettingKey() => "CompanyEvent_Insert";
    }
}