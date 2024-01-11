namespace Fanex.NexHR.ViewModels.HistoryWork
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Models;

    public class CreateHistoryWorkViewModel
    {
        public int EmpId { get; set; }
     
        public int ModifiedBy { get; set; }

        public string JobtitleName { get; set; }

        public string GroupName { get; set; }

        public int JobTitleId { get; set; }

        public int GroupId { get; set; }

        [DataType(DataType.Date)]
        public DateTime AffectedDate { get; set; }

        public Employee Employee { get; set; }

        public IList<Team> Teams { get; set; }

        public IEnumerable<JobTitle> JobTitles { get; set; }
    }
}