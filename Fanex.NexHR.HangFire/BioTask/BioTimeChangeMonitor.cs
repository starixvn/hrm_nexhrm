namespace Fanex.NexHR.HangFire.BioTask
{
    using System;
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

    public class BioTimeChangeMonitor
    {
        private readonly BaseService baseService;
        private readonly INexbotService nexbotService;
        private readonly IBioService bioService;

        private readonly DateTime today = DateTime.Now;
        private readonly int intervalMinute = 15;

        public BioTimeChangeMonitor()
        {
            baseService = new BaseService();
            nexbotService = new NexbotService();
            bioService = new BioService();
        }

        public void Initialize()
        {
            RecurringJob.AddOrUpdate(() => SendMessageToConversation(HangfireCategoryMessageType.BioTimeChange), Cron.MinuteInterval(intervalMinute));
        }

        public async Task SendMessageToConversation(HangfireCategoryMessageType categoryId)
        {
            var timeChanges = await GetWarningTimeChange();
            if (timeChanges.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation(categoryId);

                if (conversationIds.Any())
                {
                    var message = GenerateMesssage(timeChanges);
                    foreach (var conversationId in conversationIds)
                    {
                        await nexbotService.SendMessage(conversationId, message);
                    }
                }
            }
        }

        private string GenerateMesssage(IEnumerable<BioTimeChange> timeChanges)
        {
            var message = $"Hi All {Constants.NewLine}";
            message += $"There is(are) {timeChanges.Count()} time changes alert today ({today.ToString("dd MMM, yyyy")}): {Constants.NewLine} ";
            foreach (var timeChange in timeChanges)
            {
                message += $"_- Device Id: {timeChange.DeviceId}. Device Name: {timeChange.DeviceName}. Time Changed: {timeChange.TimeChange.ToShortFormatedDate()} _ {Constants.NewLine}";
            }
            message += $"Please take a look review this. {Constants.NewLine} Thanks";
            return message;
        }

        private async Task<IEnumerable<BioTimeChange>> GetWarningTimeChange()
        {
            var criteria = new GetBioTimeChangeCriteria
            {
                FromDate = new DateTime(2018, 6, 1),
                ToDate = today
            };

            var timeChanges = await bioService.GetBioTimeChangesAsync(criteria);

            return timeChanges.Where(t => t.Warning).OrderByDescending(x => x.TimeChange);
        }
    }
}