namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface ILogServiceProvider
    {
        void LogFile(string message);

        Task ErrorAsync(Exception exception);
    }
}