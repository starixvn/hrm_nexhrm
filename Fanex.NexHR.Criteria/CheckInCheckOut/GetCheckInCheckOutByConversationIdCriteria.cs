namespace Fanex.NexHR.Criteria.CheckInCheckOut
{
    using Fanex.Data.Repository;

    public class GetCheckInCheckOutByConversationIdCriteria : CriteriaBase
    {
        public string ConversationId { get; set; }
        public int EmpId { get; set; }

        public override string GetSettingKey() => "CheckInCheckOut_GetByConversationId";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ConversationId);
    }
}