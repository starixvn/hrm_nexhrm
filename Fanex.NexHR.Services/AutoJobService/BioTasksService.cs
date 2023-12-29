namespace Fanex.NexHR.Services.AutoJobService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Bio;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Bio;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;

    public class BioTasksService : IBotBioTasksService
    {
        private readonly string BaseUrl;
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;

        public BioTasksService(INexbotService nexbotService, IBaseService baseService, AppSetting appSetting)
        {
            this.nexbotService = nexbotService;
            this.baseService = baseService;
            BaseUrl = appSetting.BaseUrl + "/bio/fingerprint";
        }

        public async Task<CommandResult> SendMessageBioDeviceAsync()
        {
            var result = new CommandResult { Code = 0 };
            var pendingDevices = await baseService.GetDataAsync<BioDevice>(new GetPendingBioDevicesCriteria()).ConfigureAwait(false);
            if (pendingDevices.Any())
            {
                var conversations = await nexbotService.GetRegisteredConversationAsync(MessageCategory.BioDevice);

                if (conversations.Any())
                {
                    var message = GenerateMessageBioDevice(pendingDevices);
                    foreach (var conversation in conversations)
                    {
                        result.Code = 1;
                        await nexbotService.SendMessageAsync(conversation.ConversationId, message).ConfigureAwait(false);
                    }
                }
            }

            return result;
        }

        public async Task<CommandResult> SendMessageBioTimeChangeAsync()
        {
            var result = new CommandResult { Code = 0 };
            var criteria = new GetWarningBioTimeChangeCriteria
            {
                FromDate = DateTimeOffset.Now.Date.AddDays(-1),
                ToDate = DateTimeOffset.Now.Date.AddDays(1)
            };
            var warningTimeChanges = await baseService.GetDataAsync<BioTimeChange>(criteria).ConfigureAwait(false);

            if (warningTimeChanges.Any())
            {
                var conversations = await nexbotService.GetRegisteredConversationAsync(MessageCategory.BioTimeChange);

                if (conversations.Any())
                {
                    var message = GenerateMesssageBioTimeChange(warningTimeChanges);
                    foreach (var conversation in conversations)
                    {
                        result.Code = 1;
                        await nexbotService.SendMessageAsync(conversation.ConversationId, message);
                    }
                }
            }

            return result;
        }

        public async Task<CommandResult> SendMessageBioFingerprintAndCardAynsc()
        {
            var result = new CommandResult { Code = 0 };
            var members = await baseService.GetDataAsync<FingerprintAndCardSummary>(
                new GetPendingFingerprintAndCardCriteria()
                );

            if (members.Any())
            {
                var conversations = await nexbotService.GetRegisteredConversationAsync(MessageCategory.BioFingerprintAndCard);

                if (conversations.Any())
                {
                    result.Code = 1;
                    var message = GenerateMesssageBioFingerprintAndCard(members);
                    foreach (var conversation in conversations)
                    {
                        await nexbotService.SendMessageAsync(conversation.ConversationId, message);
                    }
                }
            }

            return result;
        }

        private static string GenerateMesssageBioTimeChange(IEnumerable<BioTimeChange> timeChanges)
        {
            var message = new StringBuilder();
            message.Append(string.Format(Messages.AnounceBioChangeTime, timeChanges.Count(), Constants.NewLine));
            foreach (var timeChange in timeChanges)
            {
                message.Append(string.Format(Messages.DeviceBioChangeTime, timeChange.DeviceId, timeChange.DeviceName, timeChange.TimeChange.ToShortFormatedDate(), Constants.NewLine));
            }
            message.Append(string.Format(Messages.ThankReview));

            return message.ToString();
        }

        private static string GenerateMessageBioDevice(IEnumerable<BioDevice> bioDevices)
        {
            StringBuilder message = new StringBuilder();
            message.Append(string.Format(Messages.AnounceDeviceInjectedBioSystem, bioDevices.Count(), Constants.NewLine));
            foreach (var device in bioDevices)
            {
                message.Append(string.Format(Messages.DeviceInjectedBioSystem, device.DeviceId, device.DeviceName, Constants.NewLine));
            }
            message.Append(string.Format(Messages.Thanks));

            return message.ToString();
        }

        private string GenerateMesssageBioFingerprintAndCard(IEnumerable<FingerprintAndCardSummary> members)
        {
            StringBuilder message = new StringBuilder();
            message.Append(string.Format(Messages.AnounceNewBioFingerprintAndCard, members.Count(), Constants.NewLine));
            foreach (var member in members)
            {
                string typeToApprove = member.IsFingerprintWarning ? Labels.FingerprintWarning : string.Empty;
                typeToApprove += member.IsFingerprintWarning && member.IsCardWarning ? Labels.AndCardWarning : string.Empty;
                typeToApprove += member.IsCardWarning && !member.IsFingerprintWarning ? Labels.CardWarning : string.Empty;
                message.Append(string.Format(Messages.EmployeeFingerprintAndCard, member.FullName, member.NickName, typeToApprove, Constants.NewLine));
            }
            message.Append(string.Format(Messages.AccessLinkBioFingerprintAndCard, BaseUrl));

            return message.ToString();
        }
    }
}