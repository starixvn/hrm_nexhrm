namespace Fanex.NexHR.Services
{
    using System.Diagnostics;
    using Fanex.NexHR.Services.Interfaces;

    public class ShellCommandService : IShellCommandService
    {
        public string ExcuteCommand(string command)
        {
            string result;
            var escapedArgs = command.Replace("\"", "\\\"");
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "/C " + escapedArgs,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = false
            };

            var proc = Process.Start(startInfo);
            result = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            return result;
        }
    }
}
