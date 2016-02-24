namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;

    public class Department
    {
        public Department()
        {
            Children = new List<Department>();
        }

        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public string ParentName { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsDisabled { get; set; }

        public int DepartmentLevel { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? DisabledDate { get; set; }

        public Employee Creator { get; set; }

        public Employee Manager { get; set; }

        public Employee Modifier { get; set; }

        public IList<Department> Children { get; set; }
    }
}
