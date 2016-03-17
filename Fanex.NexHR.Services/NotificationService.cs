using Fanex.Data.Repository;
using Fanex.NexHR.Models.Notification;
using Fanex.NexHR.Services.Interfaces;

namespace Fanex.NexHR.Services
{
    public class NotificationService : BaseService, INotificationService
    {
        public Notification GetViolationByEmpId(ICriteria criteria)
        {
            Notification noti = new Notification();
            noti = GetDataEntity<Notification>(criteria);

            return noti;
        }
    }
}
