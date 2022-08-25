namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Models;

    public interface IBotBioTasksService
    {
        Task<CommandResult> SendMessageBioDeviceAsync();

        Task<CommandResult> SendMessageBioTimeChangeAsync();

        Task<CommandResult> SendMessageBioFingerprintAndCardAynsc();
    }
}
