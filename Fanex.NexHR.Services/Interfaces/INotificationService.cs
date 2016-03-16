namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Notification;

    public interface INotificationService
    {
        Notification GetViolationByEmpId(ICriteria criteria);
    }
}
