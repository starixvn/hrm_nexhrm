namespace Fanex.NexHR.Services.Interfaces
{
    using Data.Repository;
    using Models.Notification;

    public interface INotificationService
    {
        Notification GetViolationByEmpId(ICriteria criteria);
    }
}
