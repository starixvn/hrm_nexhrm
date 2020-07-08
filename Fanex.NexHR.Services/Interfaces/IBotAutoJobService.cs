namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Models;

    public interface IBotAutoJobService
    {
        Task<CommandResult> SendMessageAsync();
    }
}