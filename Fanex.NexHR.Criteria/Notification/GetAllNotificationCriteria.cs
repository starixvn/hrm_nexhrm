namespace Fanex.NexHR.Criteria.Notification
{
    using Data.Repository;

    public class GetAllNotificationCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override bool IsValid()
        {
            return EmpId > 0;
        }

        public override string GetSettingKey()
        {
            return "Notification_GetAllViolation";
        }
    }
}
