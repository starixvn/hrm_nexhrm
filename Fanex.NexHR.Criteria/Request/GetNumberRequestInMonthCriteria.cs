namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    public class GetNumberRequestInMonthCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int RequestType { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetNumberRequestInMonth";
        }

        public override bool IsValid()
        {
            return EmpId > 0 && RequestType > 0 && FromDate <= ToDate;
        }
    }
}
