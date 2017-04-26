namespace Fanex.NexHR.Criteria.ExtensionNo
{
    using Data.Repository;

    public class AddNewExtensionNoMemberCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string ExtensionNo { get; set; }

        public bool IsInternal { get; set; }

        public int EmpId { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_Insert_EmployeeInfo";
        }

        public override bool IsValid()
        {
            return GroupId > 0;
        }
    }
}
