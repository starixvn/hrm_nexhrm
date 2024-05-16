namespace Fanex.NexHR.Criteria.SupervisorFlow
{
    using Data.Repository;

    public class GetDetailSupervisorCriteria : CriteriaBase
    {
        public string Keyword { get; set; }

         public override string GetSettingKey() => "Supervisor_Get";

        public override bool IsValid() => true;
    }
}
