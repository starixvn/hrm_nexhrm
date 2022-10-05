namespace Fanex.NexHR.Criteria.TimeSheet
{
    public class ImportDataFromBioStarCriteria : BaseCommand
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "TimeRecord_ImportData";
        }
    }
}