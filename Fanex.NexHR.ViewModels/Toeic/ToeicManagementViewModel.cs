namespace Fanex.NexHR.ViewModels.Toeic
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class ToeicManagementViewModel
    {
        public ToeicManagementViewModel()
        {
            Teams = new List<Team>();
            JobTitles = new List<JobTitle>();
        }

        public string Query { get; set; }

        public int? JobTitleId { get; set; }

        public int? TeamId { get; set; }

        public IList<Team> Teams { get; set; }

        public IList<JobTitle> JobTitles { get; set; }
    }
}