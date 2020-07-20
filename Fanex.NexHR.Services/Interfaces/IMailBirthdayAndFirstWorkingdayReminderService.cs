namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Models;

    public interface IMailBirthdayAndFirstWorkingdayReminderService
    {
        Task<CommandResult> ReminderBirthdayAndFirstWorkingday();
    }
}