namespace Fanex.NexHR.Criteria.Holiday
{
    using Fanex.Data.Repository;

    public class GetWorkDateEmployeeCalendarCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int Year { get; set; }

        public override bool IsValid()
        {
            return Year >= 2000;
        }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetWorkDate";
        }
    }
}
