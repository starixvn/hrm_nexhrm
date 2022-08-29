namespace Fanex.NexHR.Criteria.EmployeeWorkFromHome
{
    using System;

    public class UpdateEmployeeWorkFromHomeCriteria : BaseCommand
    {
        public int EmployeeWorkFromHomeId { get; set; }
        public int EmpId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ModifiedBy { get; set; }
        public string Remark { get; set; }

        public override string GetSettingKey() => "EmployeeWorkFromHome_Update";

        public override bool IsValid() => EmployeeWorkFromHomeId > 0 && EmpId > 0 && ModifiedBy > 0 && FromDate <= ToDate;
    }
}