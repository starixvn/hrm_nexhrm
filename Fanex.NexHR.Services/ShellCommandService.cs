namespace Fanex.NexHR.Services
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;

    public class ShellCommandService : IShellCommandService
    {
        public async Task<string> ExcuteCommand(string command)
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

            using var proc = Process.Start(startInfo);
            result = await proc.StandardOutput.ReadToEndAsync();
            await proc.WaitForExitAsync();

            return result;
        }
    }
}