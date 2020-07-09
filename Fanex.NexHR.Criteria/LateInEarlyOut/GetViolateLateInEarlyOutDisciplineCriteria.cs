namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using System;
    using Fanex.Data.Repository;

    public class GetViolateLateInEarlyOutDisciplineCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "LateInEarlyOut_GetLateInEarlyOutDisciplineViolate";

        public override bool IsValid() => true;
    }
}