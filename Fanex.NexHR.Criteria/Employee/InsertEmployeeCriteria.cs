namespace Fanex.NexHR.Criteria.Employee
{
    using Data;

    public class InsertEmployeeCriteria : BaseCommand
    {
        public string FullName { get; set; }

        public string NickName { get; set; }

        public string DomainUsername { get; set; }

        public string Code { get; set; }

        public string Password { get; set; }

        public int UserId { get; set; }

        public int? MonShift { get; set; }

        public int? TueShift { get; set; }

        public int? WedShift { get; set; }

        public int? ThuShift { get; set; }

        public int? FriShift { get; set; }

        public int? SatShift { get; set; }

        public int? SunShift { get; set; }

        [SpParam(SqlParamName = "GroupId")]
        public int TeamId { get; set; }

        public int CreateBy { get; set; }

        public override bool IsValid()
        {
            return TeamId > 0
                && !string.IsNullOrWhiteSpace(Code)
                && !string.IsNullOrWhiteSpace(NickName)
                && !string.IsNullOrWhiteSpace(FullName);
        }

        public override string GetSettingKey() => "Employee_Insert";
    }
}