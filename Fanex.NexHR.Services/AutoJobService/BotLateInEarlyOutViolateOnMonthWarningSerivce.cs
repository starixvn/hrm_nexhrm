namespace Fanex.NexHR.Services.AutoJobService
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Interfaces;
    using Newtonsoft.Json;

    public class BotLateInEarlyOutViolateOnMonthWarningSerivce : IBotAutoJobService
    {
        private const int LateInEarlyOutViolateInMonth = 7;
        private const string FormatDate = "MMM, yyyy";
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        private readonly IHangFireService _hangFireService;
        private readonly INexbotService _nexbotService;
        private readonly ILateInEarlyOutService _lateInEarlyOutService;
        private readonly CommandResult Result = new CommandResult { Code = 0 };

        public BotLateInEarlyOutViolateOnMonthWarningSerivce() : this(null, null, null)
        {
        }

        public BotLateInEarlyOutViolateOnMonthWarningSerivce(
            IHangFireService hangFireService,
            INexbotService nexbotService,
            ILateInEarlyOutService lateInEarlyOutService
        )
        {
            _hangFireService = hangFireService ?? new HangFireService();
            _nexbotService = nexbotService ?? new NexbotService();
            _lateInEarlyOutService = lateInEarlyOutService ?? new LateInEarlyOutService();
        }

        public async Task<CommandResult> SendMessageAsync()
        {
            var criteria = new GetLateInEarlyOutViolaterOnCurrentMonthCriteria
            {
                Month = DateTime.Now.Day == 1 ? (DateTime.Now.Month - 1) : DateTime.Now.Month,
                Year = DateTime.Now.Year
            };
            var violaters = await _lateInEarlyOutService.GetLateInEarlyOutViolateOnMonth(criteria).ConfigureAwait(false);

            if (violaters.Any())
            {
                var isChange = await _hangFireService.ObjectIsChangedOrNotExists(LateInEarlyOutViolateInMonth, violaters).ConfigureAwait(false);

                if (isChange)
                {
                    await SendMessageToViolatedEmployees(violaters.Where(v => !string.IsNullOrWhiteSpace(v.ConversationId))).ConfigureAwait(false);

                    await SendMessageToRegisterConversation(violaters.Where(v => v.TimeOnMonth >= 4)).ConfigureAwait(false);

                    await _hangFireService.UpdateJsonObject(LateInEarlyOutViolateInMonth, violaters).ConfigureAwait(false);
                }
            }

            return Result;
        }

        private async Task SendMessageToViolatedEmployees(IEnumerable<LateInEarlyOutViolateModel> violaters)
        {
            if (violaters.Any())
            {
                string jsonObject = await _hangFireService.GetJsonObject(LateInEarlyOutViolateInMonth).ConfigureAwait(false);
                await SendMessageAfterChecking(jsonObject, violaters).ConfigureAwait(false);
            }
        }

        private async Task SendMessageAfterChecking(string jsonObject, IEnumerable<LateInEarlyOutViolateModel> violaters)
        {
            if (!string.IsNullOrWhiteSpace(jsonObject))
            {
                IEnumerable<LateInEarlyOutViolateModel> oldViolaters = JsonConvert.DeserializeObject<IEnumerable<LateInEarlyOutViolateModel>>(jsonObject);
                foreach (var violater in violaters)
                {
                    if (oldViolaters.Any(o => o.EmpId == violater.EmpId && violater.TimeOnMonth != o.TimeOnMonth))
                    {
                        string message = GeneratePersonalMessage(violater);
                        await _nexbotService.SendMessageAsync(violater.ConversationId, message).ConfigureAwait(false);
                    }
                }
            }
            else
            {
                foreach (var violater in violaters)
                {
                    string message = GeneratePersonalMessage(violater);
                    await _nexbotService.SendMessageAsync(violater.ConversationId, message).ConfigureAwait(false);
                }
            }
        }

        private static string GeneratePersonalMessage(LateInEarlyOutViolateModel violater)
        {
            var msg = string.Format(Messages.DearSkypeMessage, violater.NickName, Constants.NewLine);
            if (violater.TimeOnMonth >= 4)
            {
                msg = msg + string.Format(
                    Messages.EmployeeLateInEarlyOutViolateOnMonth,
                    violater.TimeOnMonth,
                    DateTime.Now.ToString(FormatDate),
                    Constants.NewLine);
            }
            else
            {
                msg += string.Format(
                    Messages.EmployeeLateInEarlyOutViolateOnMonthOnceMoreTime,
                    violater.TimeOnMonth,
                    DateTime.Now.ToString(FormatDate),
                    Constants.NewLine);
            }

            return msg;
        }

        private async Task SendMessageToRegisterConversation(IEnumerable<LateInEarlyOutViolateModel> violaters)
        {
            var conversations = await _nexbotService.GetRegisteredConversation(LateInEarlyOutViolateInMonth).ConfigureAwait(false);
            if (conversations.Any() && violaters.Any())
            {
                string message = GenerateGroupMessage(violaters);
                foreach (var conversation in conversations)
                {
                    await _nexbotService.SendMessageAsync(conversation, message).ConfigureAwait(false);
                }
            }
        }

        private static string GenerateGroupMessage(IEnumerable<LateInEarlyOutViolateModel> violaters)
        {
            StringBuilder message = new StringBuilder();
            message.Append(string.Format(
                Messages.ThereIsNumberEmployeeViolateLateInEarlyOutOnMonth,
                violaters.Count(), DateTime.Now.ToString(FormatDate),
                Constants.NewLine));

            foreach (var violater in violaters)
            {
                message.Append(string.Format(
                    Messages.EmployeeViolateTime,
                    violater.FullName, violater.NickName,
                    violater.TimeOnMonth,
                    Constants.NewLine));
            }

            message.Append(string.Format(
                Messages.AccessLinkToViewViolateLateInEarlyOutOnMonth,
                BaseUrl));

            return message.ToString();
        }
    }
}