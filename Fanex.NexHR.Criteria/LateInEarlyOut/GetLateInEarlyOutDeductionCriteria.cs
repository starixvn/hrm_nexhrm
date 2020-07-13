namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutDeductionCriteria : CriteriaBase
    {
        private const int MinYear = 2000;

        public int Year { get; set; }

        public override string GetSettingKey() => "LateInEarlyOut_GetLateInEarlyOutToDeduct";

        public override bool IsValid() => Year > MinYear;
    }
}