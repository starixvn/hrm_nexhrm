namespace Fanex.NexHR.Models.Report
{
    public class HeadCount
    {
        public int GroupId { get; set; }

        public string GroupCode { get; set; }

        public string GroupName { get; set; }

        public int EmployeeAtJanuary { get; set; }

        public int EmployeeAtDecember { get; set; }

        public int NewHire { get; set; }

        public int Resigned { get; set; }

        public int Transfer { get; set; }

        public int Variance
        {
            get
            {
                return EmployeeAtDecember - EmployeeAtJanuary;
            }
        }
    }
}