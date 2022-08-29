namespace Fanex.NexHR.Criteria.EmployeeWorkFromHome
{
    using System;

    public class EmployeeWorkFromHomeDisApproveRequestCriteria : BaseCommand
    {
        public int WorkFromHomeRequestId { get; set; }

        public int ProcessedBy { get; set; }

        public DateTime ProcessedDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeWorkFromHome_DisApproveRequest";
        }

        public override bool IsValid()
        {
            return WorkFromHomeRequestId > 0 && ProcessedBy > 0;
        }
    }
}