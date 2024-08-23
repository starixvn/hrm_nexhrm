namespace Fanex.NexHR.Criteria.WorkFromHomeRequest
{
    using System;

    public class InsertWorkFromHomeRequestCommand : BaseCommand
    {
        public int EmpId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CreatedBy { get; set; }
        public string Remark { get; set; }

        public string HRRemark { get; set; }

        public byte StatusId { get; set; }

        public override string GetSettingKey() => "WorkFromHomeRequest_Insert";

        public override bool IsValid() => EmpId > 0 && FromDate <= ToDate && CreatedBy > 0;
    }
}