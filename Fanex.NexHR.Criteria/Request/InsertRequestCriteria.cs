namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Core.Attributes;
    using Data.Repository;

    public class InsertRequestCriteria : CriteriaBase
    {
        [Output]
        public long Id { get; set; }

        public int EmpId { get; set; }

        public int RequestType { get; set; }

        public bool IsPlanned { get; set; }

        public string Remark { get; set; }

        public DateTime FromDate { get; set; }

        public int? FromShift { get; set; }

        public DateTime? ToDate { get; set; }

        public int? ToShift { get; set; }

        public int? LeaveTypeId { get; set; }

        public decimal? TotalDays { get; set; }

        public int? LateInMins { get; set; }

        public int? EarlyOutMins { get; set; }

        public bool IsCrossDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Insert";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
