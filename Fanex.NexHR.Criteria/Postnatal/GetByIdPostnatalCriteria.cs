namespace Fanex.NexHR.Criteria.Postnatal
{
    using System;
    using Data;
    using Data.Repository;

    public class GetByIdPostnatalCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "EmployeePostnatal_GetById";
    }
}
