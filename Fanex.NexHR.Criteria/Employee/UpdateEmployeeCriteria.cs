namespace Fanex.NexHR.Criteria.Employee
{
    using Data;
    using System;

    public class UpdateEmployeeCriteria : BaseCommand
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string DomainUsername { get; set; }
        public string FullName { get; set; }
        public string EducationBackground { get; set; }
        public string NickName { get; set; }
        public string EmployeeCard { get; set; }
        [SpParam(SqlParamName = "dob")]
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public int? JobTitleId { get; set; }
        public bool Enabled { get; set; }
        public bool IsSpecialUser { get; set; }
        public string Phone { get; set; }
        public byte? Gender { get; set; }
        [SpParam(SqlParamName = "MainGroupId")]
        public int TeamId { get; set; }
        public string SubGroups { get; set; }
        public string Roles { get; set; }
        public DateTime? JoinedDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public DateTime? ResignedDate { get; set; }
        public string Skype { get; set; }
        public string Yahoo { get; set; }
        public bool Marriage { get; set; }
        public string Address { get; set; }
        public string CurrAddress { get; set; }
        public string IdCardNo { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string IssuedPlace { get; set; }
        public string PassportNo { get; set; }
        public string SocialInsuranceNo { get; set; }
        public DateTime? PassportExp { get; set; }
        public int ModifiedBy { get; set; }
        public string Geomaplatitude { get; set; }
        public string Geomaplongtitude { get; set; }
        public string Distancetocompany { get; set; }
        public string PlaceOfBirth { get; set; }
        public string UserRelationship { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactAddress { get; set; }
        public DateTime GroupDate { get; set; }
        public override bool IsValid() => IsEmployeeIdAndTeamIdValid()
                   && !string.IsNullOrWhiteSpace(Code)
                   && !string.IsNullOrWhiteSpace(NickName)
                   && !string.IsNullOrWhiteSpace(FullName);
        

        private bool IsEmployeeIdAndTeamIdValid() => EmployeeId > 0 && TeamId > 0;

        public override string GetSettingKey() => "Employee_UpdateEmployee";
    }
}