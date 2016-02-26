namespace Fanex.NexHR.Criteria.Postnatal
{
    using Fanex.Data.Repository;
    using System;

    public class GetByIdPostnatalCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeePostnatal_GetById";
        }
    }
}
