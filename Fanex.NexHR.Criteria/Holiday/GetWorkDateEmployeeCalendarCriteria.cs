namespace Fanex.NexHR.Criteria.Holiday
{
    using Fanex.Data.Repository;

    public class GetWorkDateEmployeeCalendarCriteria : CriteriaBase
    {
        private const int MinYear = 2000;
        public int EmpId { get; set; }

        public int Year { get; set; }

        public override bool IsValid()
        {
            return Year >= MinYear;
        }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetWorkDate";
        }
    }
}
