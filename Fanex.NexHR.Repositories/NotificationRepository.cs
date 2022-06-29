namespace Fanex.NexHR.Repositories
{
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.Notification;

    public class NotificationRepository : INotificationRepository
    {
        public Notification GetViolationByEmpId(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var noti = db.QueryEntity<Notification, Employee, Notification>(
                    (notification, employee) =>
                    {
                        notification.Employee = employee;
                        return notification;
                    },
                        "EmpId",
                        criteria);
                return noti;
            }
        }
    }
}