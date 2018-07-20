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

    public class BioFingerScannerMonitor : INexbotEvent
    {
        private readonly INexbotService nexbotService;
        private readonly IBioService bioService;

        private readonly int intervalMinute = ConfigurationManager.AppSettings["IntervalMiniteToSendMessage"] != null ?
                                                int.Parse(ConfigurationManager.AppSettings["IntervalMiniteToSendMessage"]) : 30;

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
            var members = await GetWarningMembers();
            if (members.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation(categoryId);

                if (conversationIds.Any())
                {
                    var message = GenerateMesssage(members);
                    foreach (var conversationId in conversationIds)
                    {
                        await nexbotService.SendMessage(conversationId, message);
                    }
                }
            }
        }

        private string GenerateMesssage(IEnumerable<FingerprintAndCardSummary> members)
        {
            var message = $"Hi All, {Constants.NewLine}";
            message += $"There are {members.Count()} member(s) need approved fingerprint or card: {Constants.NewLine}";
            foreach (var member in members)
            {
                string typeToApprove = member.IsFingerprintWarning ? "**Fingerprint**" : string.Empty;
                typeToApprove += member.IsFingerprintWarning && member.IsCardWarning ? " and **Card**" : string.Empty;
                typeToApprove += member.IsCardWarning && !member.IsFingerprintWarning ? "**Card**" : string.Empty;

                message += $"_ - {member.Employee.FullName} ({member.Employee.NickName}) need approved {typeToApprove}. _ {Constants.NewLine}";
            }
            var hrmLink = "https://hrm.nexcel.vn/fingerprintscanner";
            message += $"Please take a look at {hrmLink} to review this. {Constants.NewLine} Thanks";
            return message;
        }

        private async Task<IEnumerable<FingerprintAndCardSummary>> GetWarningMembers()
        {
            var criteria = new GetFingerprintAndCardSummaryCriteria
            {
                Status = FingerprintScannerStatus.Pending
            };

            return await bioService.GetWarningFingerprintAndCardSummaryAsync(criteria);
        }
    }
}