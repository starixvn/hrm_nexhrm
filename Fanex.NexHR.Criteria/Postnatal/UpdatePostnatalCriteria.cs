namespace Fanex.NexHR.Criteria.Postnatal
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;
    using System;

    public class UpdatePostnatalCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "OldEmpId")]
        public int OldEmployeeId { get; set; }

        [SpParam(SqlParamName = "NewEmpId")]
        public int NewEmployeeId { get; set; }

        public DateTime OldFromDate { get; set; }

        public DateTime NewFromDate { get; set; }

        public DateTime OldToDate { get; set; }

        public DateTime NewToDate { get; set; }

        public int ModifiedBy { get; set; }

        public int Type { get; set; }

        public int Minutes { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeePostnatal_Update";
        }
    }
}
