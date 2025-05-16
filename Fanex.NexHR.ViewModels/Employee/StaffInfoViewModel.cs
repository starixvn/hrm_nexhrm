namespace Fanex.NexHR.ViewModels.Employee
{
    using System.Collections.Generic;
    using Models;

    public class StaffInfoViewModel
    {
        public string Keyword { get; set; }

        public int TeamId { get; set; }

        public int DepartmentId { get; set; }

        public IList<Team> Teams { get; set; }

        public IEnumerable<Department> Departments { get; set; }
    }
}