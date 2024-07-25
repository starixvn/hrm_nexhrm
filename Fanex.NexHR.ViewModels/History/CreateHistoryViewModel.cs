namespace Fanex.NexHR.ViewModels.History
{
    using System;
    using Fanex.NexHR.Resources;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class CreateHistoryViewModel
    {
        [Required]
        [Display(Name = "Employee", ResourceType = typeof(Labels))]
        public int EmployeeId { get; set; }

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
