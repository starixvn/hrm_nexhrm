namespace Fanex.NexHR.Repositories
{
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.Notification;

    public class NotificationRepository : INotificationRepository
    {
        public async Task<Notification> GetViolationByEmpId(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var noti = await db.QueryEntityAsync<Notification, Employee, Notification>(
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