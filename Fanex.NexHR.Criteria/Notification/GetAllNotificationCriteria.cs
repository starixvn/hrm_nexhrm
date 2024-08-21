namespace Fanex.NexHR.Criteria.Notification
{
    using Data.Repository;

    public class GetAllNotificationCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override bool IsValid() => EmpId > 0;

        public override string GetSettingKey() => "Notification_GetAllViolation";
    }
}
