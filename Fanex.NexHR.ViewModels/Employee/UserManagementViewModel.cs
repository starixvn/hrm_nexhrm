namespace Fanex.NexHR.ViewModels.Employee
{
    using System.Collections.Generic;
    using Models;

    public class UserManagementViewModel
    {
        public string Keyword { get; set; }

        public int TeamId { get; set; }

        public int DepartmentId { get; set; }

        public int JobTitleId { get; set; }

        public bool IsSpecial { get; set; }

        public IEnumerable<Team> Teams { get; set; }

        public IEnumerable<Department> Departments { get; set; }

        public IEnumerable<JobTitle> JobTitles { get; set; }
    }
}