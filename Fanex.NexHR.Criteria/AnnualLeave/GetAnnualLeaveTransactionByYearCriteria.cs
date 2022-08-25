namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Fanex.Data.Repository;
    public class GetAnnualLeaveTransactionByYearCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_GetTransByYear";
        }

        public override bool IsValid()
        {
            return Year > 0;
        }
    }
}
