namespace Fanex.NexHR.Repositories.Interfaces
{
    using Data.Repository;
    using Models.Notification;
    using System.Threading.Tasks;

    public interface INotificationRepository
    {
        Task<Notification> GetViolationByEmpId(ICriteria criteria);
    }
}
