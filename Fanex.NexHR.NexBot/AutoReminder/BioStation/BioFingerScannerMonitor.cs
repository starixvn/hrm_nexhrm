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

        private readonly string intervalCronExpression = ConfigurationManager.AppSettings["BioMachineCronExpression"] ?? "0 0/12 * * 1-5";

        public BioFingerScannerMonitor()
        {
            nexbotService = new NexbotService();
            bioService = new BioService();
        }

        public void Initialize()
        {
            RecurringJob.AddOrUpdate(HangfireCategoryMessageType.BioFingerprintAndCard.GetDisplayName(),
                () => SendMessageToConversation(HangfireCategoryMessageType.BioFingerprintAndCard),
                intervalCronExpression);
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
            message.Append($"Hi All, There is/are {members.Count()} new registered fingerprint(s) and/or card(s) waiting to be approved:{Constants.NewLine}");
            foreach (var member in members)
            {
                string typeToApprove = member.IsFingerprintWarning ? "**Fingerprint**" : string.Empty;
                typeToApprove += member.IsFingerprintWarning && member.IsCardWarning ? " and **Card**" : string.Empty;
                typeToApprove += member.IsCardWarning && !member.IsFingerprintWarning ? "**Card**" : string.Empty;

                message.Append($"_ - {member.Employee.FullName} ({member.Employee.NickName})'s {typeToApprove}. _ {Constants.NewLine}");
            }
            var hrmLink = "https://hrm.nexcel.vn/fingerprintscanner";
            message.Append($"Please, access {hrmLink} to review and take appropriate action. Thanks");
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