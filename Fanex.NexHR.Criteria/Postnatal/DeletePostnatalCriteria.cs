namespace Fanex.NexHR.Criteria.Postnatal
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;
    using System;

    public class DeletePostnatalCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "EmployeePostnatal_Delete";
        }
    }
}
