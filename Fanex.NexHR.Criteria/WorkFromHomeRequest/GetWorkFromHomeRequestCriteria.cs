namespace Fanex.NexHR.Criteria.WorkFromHomeRequest
{
    using System;
    using Fanex.Data.Repository;

    public class GetWorkFromHomeRequestCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? GroupId { get; set; }

        public int? EmpId { get; set; }

        public int? StatusId { get; set; }

        public override string GetSettingKey() => "WorkFromHomeRequest_Get";

        public override bool IsValid() => ToDate >= FromDate;
    }
}
