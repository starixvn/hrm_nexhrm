namespace Fanex.NexHR.Services
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Notification;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class NotificationService : BaseService, INotificationService
    {
        private readonly INotificationRepository repositoty;

        public NotificationService() 
            : this(null) 
        {
        }

        public NotificationService(INotificationRepository repositoty)
        {
            this.repositoty = repositoty ?? new NotificationRepository();
        }

        public Notification GetViolationByEmpId(ICriteria criteria)
        {
            Notification noti = new Notification();
            noti = repositoty.GetViolationByEmpId(criteria);

            return noti;
        }
    }
}
