namespace Fanex.NexHR.Models.Report
{
    using System.Collections.Generic;

    public class LateInEarlyOutDiscipline
    {
        public LateInEarlyOutDiscipline()
        {
            LateInEarlyOutDisciplineDetails = new List<LateInEarlyOutDisciplineDetail>();
        }

        public Employee Employee { get; set; }

        public int TotalTimes { get; set; }

        public IList<LateInEarlyOutDisciplineDetail> LateInEarlyOutDisciplineDetails { get; set; }
    }
}