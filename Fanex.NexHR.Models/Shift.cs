namespace Fanex.NexHR.Models
{
    public class Shift : EntityBase<int>
    {
        public string BeginCheckInTime { get; set; }

        public string EndCheckInTime { get; set; }

        public string BeginCheckOutTime { get; set; }

        public string EndCheckOutTime { get; set; }

        public int BreakTimeMins { get; set; }

        public string DisplayName => BeginCheckInTime != null && EndCheckInTime != null
            ? (BeginCheckInTime.Equals(EndCheckInTime)
                ? $"{Name} (IN {BeginCheckInTime} -> OUT:{BeginCheckOutTime})"
                : $"{Name} (IN {BeginCheckInTime} - {EndCheckInTime} -> OUT {BeginCheckOutTime} - {EndCheckOutTime})"
            )
            : string.Empty;
    }
}