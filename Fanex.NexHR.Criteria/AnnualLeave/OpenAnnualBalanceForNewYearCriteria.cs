namespace Fanex.NexHR.Criteria.AnnualLeave
{
    public class OpenAnnualBalanceForNewYearCriteria : BaseCommand
    {
        public int Year { get; set; }

        public int? EmpId { get; set; }

        public string Description { get; set; }

        public override string GetSettingKey()
        {
            return "DBA_AnnualLeave_CreateAnnualBalance";
        }

        public override bool IsValid()
        {
            const short minValidYear = 2010;
            return Year > minValidYear;
        }
    }
}