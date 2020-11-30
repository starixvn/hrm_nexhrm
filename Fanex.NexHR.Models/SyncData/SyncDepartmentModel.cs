namespace Fanex.NexHR.Models.SyncData
{
    using System;

    public class SyncDepartmentModel
    {
        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public int? ManagerId { get; set; }

        public int? DepartmentLevel { get; set; }

        public bool IsActive { get; set; }

        public int? Left { get; set; }

        public int? Right { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}