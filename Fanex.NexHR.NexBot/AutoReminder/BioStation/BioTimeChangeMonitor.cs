namespace Fanex.NexHR.NexBot.AutoReminder.BioStation
{
    using System;
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

    public class BioTimeChangeMonitor : INexbotEvent
    {
        private readonly INexbotService nexbotService;
        private readonly IBioService bioService;

        private readonly DateTime today = DateTimeOffset.Now.DateTime.ToClientTimeZone();

        private readonly string intervalCronExpression = ConfigurationManager.AppSettings["BioMachineCronExpression"] ?? "0 0/12 * * 1-5";

        public BioTimeChangeMonitor()
        {
            nexbotService = new NexbotService();
            bioService = new BioService();
        }

        public void Initialize()
        {
            RecurringJob.AddOrUpdate(HangfireCategoryMessageType.BioTimeChange.GetDisplayName(),
                () => SendMessageToConversation(HangfireCategoryMessageType.BioTimeChange),
                intervalCronExpression);
        }

        public async Task SendMessageToConversation(HangfireCategoryMessageType categoryId)
        {
            var timeChanges = await GetWarningTimeChange().ConfigureAwait(false);
            if (timeChanges.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation(categoryId).ConfigureAwait(false);

                if (conversationIds.Any())
                {
                    var message = GenerateMesssage(timeChanges);
                    foreach (var conversationId in conversationIds)
                    {
                        await nexbotService.SendMessage(conversationId, message).ConfigureAwait(false);
                    }
                }
            }
        }

        private string GenerateMesssage(IEnumerable<BioTimeChange> timeChanges)
        {
            var message = new StringBuilder();
            message.Append($"Hi All, There is/are {timeChanges.Count()} change(s) of time, today ({today.ToString("dd MMM, yyyy")}): {Constants.NewLine}");
            foreach (var timeChange in timeChanges)
            {
                message.Append($"_- Device Id: {timeChange.DeviceId}. Device Name: {timeChange.DeviceName}. Time Changed: {timeChange.TimeChange.ToShortFormatedDate()}. _ {Constants.NewLine}");
            }
            message.Append($"Please, review the change(s). Thanks");
            return message.ToString();
        }

        private async Task<IEnumerable<BioTimeChange>> GetWarningTimeChange()
        {
            var criteria = new GetBioTimeChangeCriteria
            {
                FromDate = new DateTime(2018, 6, 1),
                ToDate = today
            };

            var timeChanges = await bioService.GetBioTimeChangesAsync(criteria).ConfigureAwait(false);

            return timeChanges.Where(t => t.Warning).OrderByDescending(x => x.TimeChange);
        }
    }
}