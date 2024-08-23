namespace Fanex.NexHR.Criteria.WorkFromHomeRequest
{
    using System;

    public class UpdateWorkFromHomeRequestByHRCommand : BaseCommand
    {
        public int WorkFromHomeRequestId { get; set; }
        public int EmpId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ModifiedBy { get; set; }
        public string HRRemark { get; set; }

        public override string GetSettingKey() => "WorkFromHomeRequest_UpdateByHR";

        public override bool IsValid() => WorkFromHomeRequestId > 0 && EmpId > 0 && FromDate <= ToDate && ModifiedBy > 0;
    }
}