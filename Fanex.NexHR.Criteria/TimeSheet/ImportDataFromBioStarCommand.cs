namespace Fanex.NexHR.Criteria.TimeSheet;

public class ImportDataFromBioStarCommand : BaseCommand
{
    public string TimeRecordXml { get; set; }

    public override bool IsValid() => true;

    public override string GetSettingKey() => "TimeRecord_ImportData";
}