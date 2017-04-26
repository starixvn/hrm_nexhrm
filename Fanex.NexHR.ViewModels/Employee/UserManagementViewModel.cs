namespace Fanex.NexHR.ViewModels.Employee
{
    using System.Collections.Generic;
    using Models;

    public class UserManagementViewModel
    {
        public string Keyword { get; set; }

        public int TeamId { get; set; }

        public int JobTitleId { get; set; }

        public IList<Team> Teams { get; set; }

        public IList<JobTitle> JobTitles { get; set; }
    }
}