namespace Fanex.NexHR.Criteria.ExtensionNo
{
    public class AddNewExternalGroupCriteria : BaseCommand
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