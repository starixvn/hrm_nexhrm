namespace Fanex.NexHR.Criteria.ExtensionNo
{
    using Fanex.Data.Repository;

    public class AddNewExternalGroupCriteria : CriteriaBase
    {
        public string FullName { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_Insert_GroupExternal";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
