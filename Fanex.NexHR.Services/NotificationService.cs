namespace Fanex.NexHR.Services
{
    using Data.Repository;
    using Interfaces;
    using Models.Notification;
    using Repositories;
    using Repositories.Interfaces;

    public class NotificationService : BaseService, INotificationService
    {
        private readonly INotificationRepository _repositoty;

        public NotificationService() 
            : this(null) 
        {
        }

        public NotificationService(INotificationRepository repositoty)
        {
            _repositoty = repositoty ?? new NotificationRepository();
        }

        public Notification GetViolationByEmpId(ICriteria criteria)
        {
            return _repositoty.GetViolationByEmpId(criteria);
        }
    }
}
