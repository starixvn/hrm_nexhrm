namespace Fanex.NexHR.Models.Report
{
    using System.Collections.Generic;

    public class LateInEarlyOutByTeamModel
    {
        public Team Team { get; set; }

        public List<LateInEarlyOutModel> LateInEarlyOutSummaryInTeam { get; set; }
    }
}
