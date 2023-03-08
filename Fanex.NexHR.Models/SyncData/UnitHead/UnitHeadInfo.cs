namespace Fanex.NexHR.Models.SyncData
{
    using System;

    [Serializable]
    public class UnitHeadInfo
    {
        public int EmpId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
    }
}