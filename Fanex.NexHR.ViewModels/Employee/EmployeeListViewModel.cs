namespace Fanex.NexHR.ViewModels.Employee
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class EmployeeListViewModel
    {
        public IList<Employee> Employees { get; set; }

        public int Type { get; set; }
    }

    public enum TypeList
    {
        Active = 0,
        Disabled = 1,
        Deleted = 2,
    }
}
