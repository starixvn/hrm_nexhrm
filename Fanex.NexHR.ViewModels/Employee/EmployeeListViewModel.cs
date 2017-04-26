namespace Fanex.NexHR.ViewModels.Employee
{
    using System.Collections.Generic;
    using Models;

    public enum TypeList
    {
        Active = 0,
        Disabled = 1,
        Deleted = 2,
    }

    public class EmployeeListViewModel
    {
        public IList<Employee> Employees { get; set; }

        public int StatusViewType { get; set; }
    }
}
