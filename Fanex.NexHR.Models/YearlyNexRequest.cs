namespace Fanex.NexHR.Models
{
    public class YearlyNexRequest : TotalOfEachMonth
    {
        public float TotalHour { get; set; }

        public Employee Employee { get; set; }

        public int IsHoliday { get; set; }

        public int IsOffDay { get; set; }

        public int IsNormalDay { get; set; }
    }
}
