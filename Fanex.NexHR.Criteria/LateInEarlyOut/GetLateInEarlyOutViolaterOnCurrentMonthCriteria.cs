namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using System;
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutViolaterOnCurrentMonthCriteria : CriteriaBase
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public override string GetSettingKey() => "LateInEarlyOut_GetLateInEarlyOutOnMonth";

        public override bool IsValid() => Year <= DateTime.Now.Year;
    }
}