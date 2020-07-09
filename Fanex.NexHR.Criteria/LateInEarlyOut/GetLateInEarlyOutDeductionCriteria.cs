namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutDeductionCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey() => "LateInEarlyOut_GetLateInEarlyOutToDeduct";

        public override bool IsValid() => Year > 2000;
    }
}