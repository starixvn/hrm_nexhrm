namespace Fanex.NexHR.ViewModels.Employee
{
    public class EmployeeListFilterViewModel
    {
        public string Query { get; set; }
        public int? TeamId { get; set; }
        public int? DepartmentId { get; set; }
        public bool? IsDisable { get; set; }
        public bool? IsDelete { get; set; }
        public int? JobTitleId { get; set; }
        public string Sort { get; set; }
        public bool? IsSpecial { get; set; }
    }
}