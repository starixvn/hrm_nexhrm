namespace Fanex.NexHR.Criteria.Postnatal
{
    using System;
    using Data;

    public class InsertPostnatalCriteria : BaseCommand
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int ModifiedBy { get; set; }

        public int Minutes { get; set; }

        [SpParam(SqlParamName = "Type")]
        public int Types { get; set; }

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