namespace Fanex.NexHR.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Notification;
    using Fanex.NexHR.Repositories.Interfaces;

    public class NotificationRepository : INotificationRepository
    {
        public Notification GetViolationByEmpId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var noti = db.QueryEntity<Notification, Employee, Notification>(
                    (notification, employee) =>
                    {
                        notification.Employee = employee;
                        return notification;
                    },
                        "EmpId",
                        criteria.GetSpParams());
                return noti;
            }
        }
    }
}