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

    public class BioFingerScannerMonitor : INexbotEvent
    {
        private readonly INexbotService nexbotService;
        private readonly IBioService bioService;

        private readonly int intervalMinute = ConfigurationManager.AppSettings["BioMonitorIntervalMinute"] != null ?
                                                int.Parse(ConfigurationManager.AppSettings["BioMonitorIntervalMinute"]) : 30;

        public BioFingerScannerMonitor()
        {
            nexbotService = new NexbotService();
            bioService = new BioService();
        }

        public void Initialize()
        {
            RecurringJob.AddOrUpdate(() => SendMessageToConversation(HangfireCategoryMessageType.BioFingerprintAndCard), Cron.MinuteInterval(intervalMinute));
        }

        public async Task SendMessageToConversation(HangfireCategoryMessageType categoryId)
        {
            var members = await GetWarningMembers().ConfigureAwait(false);
            if (members.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation(categoryId).ConfigureAwait(false);

                if (conversationIds.Any())
                {
                    var message = GenerateMesssage(members);
                    foreach (var conversationId in conversationIds)
                    {
                        await nexbotService.SendMessage(conversationId, message).ConfigureAwait(false);
                    }
                }
            }
        }

        private static string GenerateMesssage(IEnumerable<FingerprintAndCardSummary> members)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Hi All, {Constants.NewLine}");
            message.Append($"There is (are) {members.Count()} member(s) need approved fingerprint or card: {Constants.NewLine}");
            foreach (var member in members)
            {
                string typeToApprove = member.IsFingerprintWarning ? "**Fingerprint**" : string.Empty;
                typeToApprove += member.IsFingerprintWarning && member.IsCardWarning ? " and **Card**" : string.Empty;
                typeToApprove += member.IsCardWarning && !member.IsFingerprintWarning ? "**Card**" : string.Empty;

                message.Append($"_ - {member.Employee.FullName} ({member.Employee.NickName}) need approved {typeToApprove}. _ {Constants.NewLine}");
            }
            var hrmLink = "https://hrm.nexcel.vn/fingerprintscanner";
            message.Append($"Please take a look at {hrmLink} to review this. {Constants.NewLine}Thanks");
            return message.ToString();
        }

        private async Task<IEnumerable<FingerprintAndCardSummary>> GetWarningMembers()
        {
            var criteria = new GetFingerprintAndCardSummaryCriteria
            {
                Status = FingerprintScannerStatus.Pending
            };

            return await bioService.GetWarningFingerprintAndCardSummaryAsync(criteria).ConfigureAwait(false);
        }
    }
}