namespace Fanex.NexHR.Criteria.Postnatal
{
    using System;
    using Data;
    using Data.Repository;

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

        [SpParam(SqlParamName = "Type")]
        public int Types { get; set; }

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
