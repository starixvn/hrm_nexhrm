namespace Fanex.NexHR.Criteria.Holiday
{
    using Data.Repository;

    public class GetHolidayByYearCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Holiday_GetByYear";
        }

        public override bool IsValid()
        {
            const int minYear = 2010;

            return Year >= minYear;
        }
    }
}
