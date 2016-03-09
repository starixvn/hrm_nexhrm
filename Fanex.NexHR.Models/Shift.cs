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
                return BeginCheckInTime.Equals(EndCheckInTime)
                    ? string.Format("{0} (IN {1} -> OUT:{2})", Name, BeginCheckInTime, BeginCheckOutTime)
                    : string.Format("{0} (IN {1} - {2} -> OUT {3} - {4})", Name, BeginCheckInTime, EndCheckInTime, BeginCheckOutTime, EndCheckOutTime);
            }
        }
    }
}
