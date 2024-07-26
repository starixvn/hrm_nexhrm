namespace Fanex.NexHR.ViewModels.History
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class CreateHistoryViewModel
    {
        public int EmpId { get; set; }

        public string Employee { get; set; }

        public int? GroupId { get; set; }

        public int? JobTitleId { get; set; }

        public int? JobGradeId { get; set; }

        public DateTime AffectedDate { get; set; }

        public IList<Team> Teams { get; set; }

        public IList<JobGrade> JobGrades { get; set; }

        public IEnumerable<JobTitle> JobTitles { get; set; }
    }
}
