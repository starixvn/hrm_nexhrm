namespace Fanex.NexHR.Criteria.WorkFromHomeRequest
{
    using System;
    using Fanex.Data.Repository;

    public class GetWorkFromHomeRequestByEmpIdCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int EmpId { get; set; }

        public override string GetSettingKey() => "WorkFromHomeRequest_Get";

        public override bool IsValid() => EmpId > 0 && ToDate >= FromDate;
    }
}
