namespace Fanex.NexHR.Criteria.SkypeAccount
{
    using Fanex.Data.Repository;

    public class UpdateSkypeAccountNickNameCriteria : CriteriaBase
    {
        public string NickName { get; set; }

        public string ConversationId { get; set; }

        public override string GetSettingKey() => "SkypeAccount_UpdateNickName";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(NickName) && !string.IsNullOrWhiteSpace(ConversationId);
    }
}