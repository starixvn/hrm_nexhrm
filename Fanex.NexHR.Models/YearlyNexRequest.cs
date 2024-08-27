namespace Fanex.NexHR.Models
{
    public class YearlyNexRequest : TotalOfEachMonth
    {
        public float TotalHour { get; set; }

        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public int TeamId { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public int IsHoliday { get; set; }

        public int IsOffDay { get; set; }

        public int IsNormalDay { get; set; }
    }
}
