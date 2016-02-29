namespace Fanex.NexHR.Criteria.Postnatal
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;
    using System;

    public class InsertPostnatalCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int ModifiedBy { get; set; }

        public int Minutes { get; set; }

        public int Type { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "EmployeePostnatal_Insert";
        }
    }
}
