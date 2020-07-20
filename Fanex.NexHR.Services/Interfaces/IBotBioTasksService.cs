namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Models;
    using System.Threading.Tasks;

    public interface IBotBioTasksService
    {
        Task<CommandResult> SendMessageBioDeviceAsync();

        Task<CommandResult> SendMessageBioTimeChangeAsync();

        Task<CommandResult> SendMessageBioFingerprintAndCardAynsc();
    }
}
