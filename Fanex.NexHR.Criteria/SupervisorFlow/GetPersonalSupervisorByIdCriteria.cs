namespace Fanex.NexHR.Criteria.SupervisorFlow
{
    using Fanex.Data.Repository;

    public class GetPersonalSupervisorByIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey() => "EmployeeSupervisor_GetById";

        public override bool IsValid() => EmpId > 0;
    }
}
