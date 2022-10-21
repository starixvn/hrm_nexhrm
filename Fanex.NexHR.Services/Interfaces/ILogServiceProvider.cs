namespace Fanex.NexHR.Services.Interfaces
{
    using System;

    public interface ILogServiceProvider
    {
        void LogFile(string message);

        void LogException(Exception exception);
    }
}