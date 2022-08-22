namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IShellCommandService
    {
        Task<string> ExcuteCommand(string command);
    }
}