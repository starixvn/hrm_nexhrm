namespace Fanex.NexHR.Models
{
    public class Shift : EntityBase<int>
    {
        public string BeginCheckInTime { get; set; }

        public string EndCheckInTime { get; set; }

        public string BeginCheckOutTime { get; set; }

        public string EndCheckOutTime { get; set; }

        public int BreakTimeMins { get; set; }

        public string DisplayName
        {
            get
            {
                if (BeginCheckInTime != null && EndCheckInTime != null)
                {
                    if (BeginCheckInTime.Equals(EndCheckInTime))
                    {
                        return $"{Name} (IN {BeginCheckInTime} -> OUT:{BeginCheckOutTime})";
                    }
                    return $"{Name} (IN {BeginCheckInTime} - {EndCheckInTime} -> OUT {BeginCheckOutTime} - {EndCheckOutTime})";
                }
                return string.Empty;
            }
        }
    }
}