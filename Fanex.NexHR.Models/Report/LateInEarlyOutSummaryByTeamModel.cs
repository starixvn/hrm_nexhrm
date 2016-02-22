namespace Fanex.NexHR.Models.Report
{
    using System.Collections.Generic;

    public class LateInEarlyOutSummaryByTeamModel
    {
        public Team Team { get; set; }

        public IList<LateInEarlyOutSummaryModel> LateInEarlyOutSummaryInTeams { get; set; }
    }
}