namespace Fanex.NexHR.NexBot.AutoReminder.BioStation
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Criteria.Bio;
    using Hangfire;
    using Interfaces;
    using Models.Bio;
    using Services;
    using Services.Interfaces;

    public class BioDeviceMonitor : INexbotEvent
    {
        private readonly BaseService baseService;
        private readonly INexbotService nexbotService;

        private readonly int intervalMinute = ConfigurationManager.AppSettings["BioMonitorIntervalMinute"] != null ?
                                                int.Parse(ConfigurationManager.AppSettings["BioMonitorIntervalMinute"]) : 30;

        public BioDeviceMonitor()
        {
            baseService = new BaseService();
            nexbotService = new NexbotService();
        }

        public void Initialize()
        {
            RecurringJob.AddOrUpdate(() => SendMessageToConversation(HangfireCategoryMessageType.BioDevice), Cron.MinuteInterval(intervalMinute));
        }

        public async Task SendMessageToConversation(HangfireCategoryMessageType categoryId)
        {
            var pendingDevices = await GetPendingBioDevices().ConfigureAwait(false);
            if (pendingDevices.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation(categoryId).ConfigureAwait(false);

                if (conversationIds.Any())
                {
                    var message = GenerateMessage(pendingDevices);
                    foreach (var conversationId in conversationIds)
                    {
                        await nexbotService.SendMessage(conversationId, message).ConfigureAwait(false);
                    }
                }
            }
        }

        private async Task<IEnumerable<BioDevice>> GetPendingBioDevices()
        {
            var bioDevices = await baseService.GetDataAsync<BioDevice>(new GetBioDevicesCriteria()).ConfigureAwait(false);

            return bioDevices.Where(x => x.StatusId == "1");
        }

        private static string GenerateMessage(IEnumerable<BioDevice> bioDevices)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Hi All, {Constants.NewLine}");
            message.Append($"There is (are) {bioDevices.Count()} device(s) just injected into the BIO system: {Constants.NewLine}");
            foreach (var device in bioDevices)
            {
                message.Append($"_ - Device Id: {device.DeviceId}. Device Name: {device.DeviceName } _ {Constants.NewLine}");
            }
            message.Append($"Please take a look review this. {Constants.NewLine}Thanks");
            return message.ToString();
        }
    }
}