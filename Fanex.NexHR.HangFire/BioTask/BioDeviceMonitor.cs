namespace Fanex.NexHR.HangFire.BioTask
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Criteria.Bio;
    using Hangfire;
    using NexHR.Models.Bio;
    using Services;
    using Services.Interfaces;

    public class BioDeviceMonitor
    {
        private readonly BaseService baseService;
        private readonly INexbotService nexbotService;
        private readonly int intervalMinite = 15;

        public BioDeviceMonitor()
        {
            baseService = new BaseService();
            nexbotService = new NexbotService();
        }

        public void Initialize()
        {
            RecurringJob.AddOrUpdate(() => SendMessageToConversation(HangfireCategoryMessageType.BioDevice), Cron.MinuteInterval(intervalMinite));
        }

        public async Task SendMessageToConversation(HangfireCategoryMessageType categoryId)
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
            var message = $"Hi All: {Constants.NewLine}";
            message += $"There is(are) {bioDevices.Count()} device(s) just injected into the BIO system: {Constants.NewLine}";
            foreach (var device in bioDevices)
            {
                message += $"* - Device Id: {device.DeviceId}. Device Name: {device.DeviceName } * {Constants.NewLine}";
            }
            message += $"Please take a look review this. {Constants.NewLine} Thanks";
            return message;
        }
    }
}