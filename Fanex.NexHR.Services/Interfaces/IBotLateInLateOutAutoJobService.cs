namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Models;
    using System.Threading.Tasks;

    public interface IBotLateInLateOutAutoJobService
    {
        Task<CommandResult> SendMessageViolateOnMonthAsync();

        Task<CommandResult> SendMessageViolateDisciplineAsync();

        Task<CommandResult> SendMessageDeductAsync();
        Task<CommandResult> DeductAsync();
        Task<CommandResult> SendMessageReminderLateInEarlyOut();
    }
}