namespace Fanex.NexHR.HangFire.BioTask
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Criteria.Bio;
    using Hangfire;
    using NexHR.Models.Bio;
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
            RecurringJob.AddOrUpdate(() => SendMessageToConversation(CategoryMessageType.BioDevice), Cron.MinuteInterval(intervalMinute));
        }

        public async Task SendMessageToConversation(CategoryMessageType categoryId)
        {
            var pendingDevices = await GetPendingBioDevices();
            if (pendingDevices.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation(categoryId);

                if (conversationIds.Any())
                {
                    var message = GenerateMessage(pendingDevices);
                    foreach (var conversationId in conversationIds)
                    {
                        await nexbotService.SendMessage(conversationId, message);
                    }
                }
            }
        }

        private async Task<IEnumerable<BioDevice>> GetPendingBioDevices()
        {
            var bioDevices = await baseService.GetDataAsync<BioDevice>(new GetBioDevicesCriteria());

            return bioDevices.Where(x => x.StatusId == "1");
        }

        private string GenerateMessage(IEnumerable<BioDevice> bioDevices)
        {
            var message = $"Hi All, {Constants.NewLine}";
            message += $"There is (are) {bioDevices.Count()} device(s) just injected into the BIO system: {Constants.NewLine}";
            foreach (var device in bioDevices)
            {
                message += $"_ - Device Id: {device.DeviceId}. Device Name: {device.DeviceName } _ {Constants.NewLine}";
            }
            message += $"Please take a look review this. {Constants.NewLine}Thanks";
            return message;
        }
    }
}