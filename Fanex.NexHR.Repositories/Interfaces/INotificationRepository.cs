namespace Fanex.NexHR.Repositories.Interfaces
{
    using Data.Repository;
    using Models.Notification;

    public interface INotificationRepository
    {
        Notification GetViolationByEmpId(ICriteria criteria);
    }
}
