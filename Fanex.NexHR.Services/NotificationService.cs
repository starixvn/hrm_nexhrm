namespace Fanex.NexHR.Services
{
    using System.Threading.Tasks;
    using Data.Repository;
    using Interfaces;
    using Models.Notification;
    using Repositories.Interfaces;

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository repositoty;

        public NotificationService(INotificationRepository repositoty)
        {
            this.repositoty = repositoty;
        }

        public async Task<Notification> GetViolationByEmpId(ICriteria criteria)
        {
            return await repositoty.GetViolationByEmpId(criteria);
        }
    }
}