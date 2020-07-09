namespace Fanex.NexHR.Controllers.API
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Bio;
    using Fanex.NexHR.Models.Bio;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class BioTasksController : ApiController
    {
        private readonly BaseService baseService;
        private readonly INexbotService nexbotService;

        public BioTasksController()
        {
            baseService = new BaseService();
            nexbotService = new NexbotService();
        }

        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> BioDevice()
        {
            var pendingDevices = await baseService.GetDataAsync<BioDevice>(new GetPendingBioDevicesCriteria()).ConfigureAwait(false);
            if (pendingDevices.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation((int)MessageCategory.BioDevice).ConfigureAwait(false);

                if (conversationIds.Any())
                {
                    var message = GenerateMessageBioDevice(pendingDevices);
                    foreach (var conversationId in conversationIds)
                    {
                        nexbotService.SendMessage(conversationId, message);
                    }
                }
            }

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> BioTimeChange()
        {
            var criteria = new GetWarningBioTimeChangeCriteria
            {
                FromDate = DateTime.Now.Date.AddDays(-1),
                ToDate = DateTime.Now.Date.AddDays(1)
            };
            var warningTimeChanges = await baseService.GetDataAsync<BioTimeChange>(criteria).ConfigureAwait(false);

            if (warningTimeChanges.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation((int)MessageCategory.BioTimeChange).ConfigureAwait(false);

                if (conversationIds.Any())
                {
                    var message = GenerateMesssageBioTimeChange(warningTimeChanges);
                    foreach (var conversationId in conversationIds)
                    {
                        nexbotService.SendMessage(conversationId, message);
                    }
                }
            }

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> BioFingerprintAndCard()
        {
            var members = await baseService.GetDataAsync<FingerprintAndCardSummary>(
                new GetPendingFingerprintAndCardCriteria()
                ).ConfigureAwait(false);

            if (members.Any())
            {
                var conversationIds = await nexbotService.GetRegisteredConversation((int)MessageCategory.BioFingerprintAndCard).ConfigureAwait(false);

                if (conversationIds.Any())
                {
                    var message = GenerateMesssageBioFingerprintAndCard(members);
                    foreach (var conversationId in conversationIds)
                    {
                        nexbotService.SendMessage(conversationId, message);
                    }
                }
            }

            return Ok();
        }

        private static string GenerateMesssageBioTimeChange(IEnumerable<BioTimeChange> timeChanges)
        {
            var message = new StringBuilder();
            message.Append($"Hi All, There is/are {timeChanges.Count()} change(s) of time: {Constants.NewLine}");
            foreach (var timeChange in timeChanges)
            {
                message.Append($"_- Device Id: {timeChange.DeviceId}. Device Name: {timeChange.DeviceName}. Time Changed: {timeChange.TimeChange.ToShortFormatedDate()}._ {Constants.NewLine}");
            }
            message.Append($"Please, review the change(s). Thanks");
            return message.ToString();
        }

        private static string GenerateMessageBioDevice(IEnumerable<BioDevice> bioDevices)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Hi All, There is/are {bioDevices.Count()} device(s) injected into the BIO system: {Constants.NewLine}");
            foreach (var device in bioDevices)
            {
                message.Append($"_- Device Id: {device.DeviceId}. Device Name: {device.DeviceName }_ {Constants.NewLine}");
            }
            message.Append($"Please review this. Thanks");
            return message.ToString();
        }

        private static string GenerateMesssageBioFingerprintAndCard(IEnumerable<FingerprintAndCardSummary> members)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Hi All, There is/are {members.Count()} new registered fingerprint(s) and/or card(s) waiting to be approved:{Constants.NewLine}");
            foreach (var member in members)
            {
                string typeToApprove = member.IsFingerprintWarning ? "**Fingerprint**" : string.Empty;
                typeToApprove += member.IsFingerprintWarning && member.IsCardWarning ? " and **Card**" : string.Empty;
                typeToApprove += member.IsCardWarning && !member.IsFingerprintWarning ? "**Card**" : string.Empty;

                message.Append($"_- {member.FullName} ({member.NickName})'s {typeToApprove}._ {Constants.NewLine}");
            }
            const string hrmLink = "https://hrm.nexcel.vn/bio/fingerprint";
            message.Append($"Please, access {hrmLink} to review and take appropriate action. Thanks");
            return message.ToString();
        }
    }
}