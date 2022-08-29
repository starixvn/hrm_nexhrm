namespace Fanex.NexHR.Criteria.EmployeeWorkFromHome
{
    using System;

    public class EmployeeWorkFromHomeApproveAndRejectRequestCriteria : BaseCommand
    {
        public int WorkFromHomeRequestId { get; set; }

        public byte StatusId { get; set; }

        public int ProcessedBy { get; set; }

        public DateTime ProcessedDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeWorkFromHome_ApproveAndRejectRequest";
        }

        public override bool IsValid()
        {
            return WorkFromHomeRequestId > 0 && StatusId > 0 && ProcessedBy > 0;
        }
    }
}