namespace Fanex.NexHR.ViewModels.EmployeeExperience
{
    using System;

    public class EmployeeExperienceInsertViewModel
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public string Employee { get; set; }
        public int? DepartmentId { get; set; }
        public string Department { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

    }
}
