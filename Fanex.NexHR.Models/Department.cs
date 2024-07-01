namespace Fanex.NexHR.Models
{
    using System;

    public class Department : EntityBase<int>
    {
        public DateTime? ModifiedDate { get; set; }

        public bool IsDisabled { get; set; }

        public int DepartmentLevel { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? DisabledDate { get; set; }
    }
}
