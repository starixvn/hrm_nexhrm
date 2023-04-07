namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class GetEmployeeConversationByIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }
        public override string GetSettingKey() => "EmployeeConversation_GetById";

        public override bool IsValid() => true;
    }
}
