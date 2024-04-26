namespace Fanex.NexHR.Criteria.SupervisorFlow
{
    using Data.Repository;

    public class GetPersonalSupervisorCriteria : CriteriaBase
    {
         public override string GetSettingKey() => "EmployeeSupervisor_Get";

        public override bool IsValid() => true;
    }
}
