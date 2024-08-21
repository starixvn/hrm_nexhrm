namespace Fanex.NexHR.Criteria.AnnualLeave
{
    public class InsertAnnualBalanceForNewYearCommand : BaseCommand
    {
        const short minValidYear = 2010;

        public int Year { get; set; }

        public int? EmpId { get; set; }

        public string Description { get; set; }

        public override string GetSettingKey() => "DBA_AnnualLeave_CreateAnnualBalance";

        public override bool IsValid() => Year > minValidYear;
    }
}