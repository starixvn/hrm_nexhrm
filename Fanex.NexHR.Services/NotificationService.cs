namespace Fanex.NexHR.Services
{
    using Data.Repository;
    using Interfaces;
    using Models.Notification;
    using Repositories.Interfaces;

    public class NotificationService : BaseService, INotificationService
    {
        private readonly INotificationRepository repositoty;

        public NotificationService(INotificationRepository repositoty)
        {
            this.repositoty = repositoty;
        }

        public Notification GetViolationByEmpId(ICriteria criteria)
        {
            return repositoty.GetViolationByEmpId(criteria);
        }
    }
}
