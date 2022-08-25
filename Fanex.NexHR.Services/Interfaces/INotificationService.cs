namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;
    using Data.Repository;
    using Models.Notification;

    public interface INotificationService
    {
        Task<Notification> GetViolationByEmpId(ICriteria criteria);
    }
}
