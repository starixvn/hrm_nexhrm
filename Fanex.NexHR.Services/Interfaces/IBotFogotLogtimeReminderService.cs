namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Models;

    public interface IBotFogotLogtimeReminderService
    {
        Task<CommandResult> SendMessageForgotLogtimeAsync();
    }
}
