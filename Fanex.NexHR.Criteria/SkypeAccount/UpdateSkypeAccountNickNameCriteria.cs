namespace Fanex.NexHR.Criteria.SkypeAccount
{
    using Fanex.Data.Repository;

    public class UpdateSkypeAccountNickNameCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string ConversationId { get; set; }

        public override string GetSettingKey() => "Employee_UpdateSkypeAccount";

        public override bool IsValid() => EmployeeId > 0 && !string.IsNullOrWhiteSpace(ConversationId);
    }
}