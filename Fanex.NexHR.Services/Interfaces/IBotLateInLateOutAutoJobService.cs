namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Models;

    public interface IBotLateInLateOutAutoJobService
    {
        Task<CommandResult> SendMessageViolateOnMonthAsync();

        Task<CommandResult> SendMessageViolateDisciplineAsync();

        Task<CommandResult> SendMessageDeductAsync();
    }
}