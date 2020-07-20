namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Models;
    using System.Threading.Tasks;

    public interface IBotFogotLogtimeReminderService
    {
        Task<CommandResult> SendMessageForgotLogtimeAsync();
    }
}
