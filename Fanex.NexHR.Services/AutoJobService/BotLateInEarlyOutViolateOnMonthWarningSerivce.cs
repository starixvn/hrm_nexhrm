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

    public class BotLateInEarlyOutViolateOnMonthWarningSerivce : IBotLateInLateOutAutoJobService
    {
        private const int LateInEarlyOutViolateOnMonth = 7;
        private const int ViolateLateInEarlyOutDiscipline = 8;
        private const string FormatShortDate = "MMM, yyyy";
        private const string FormatLongDate = "dd MMM, yyyy";
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        private readonly IHangFireService _hangFireService;
        private readonly INexbotService _nexbotService;
        private readonly ILateInEarlyOutService _lateInEarlyOutService;

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

        public async Task<CommandResult> SendMessageViolateOnMonthAsync()
        {
            var result = new CommandResult { Code = 0 };
            var criteria = new GetLateInEarlyOutViolaterOnCurrentMonthCriteria
            {
                Month = DateTime.Now.Day == 1 ? (DateTime.Now.Month - 1) : DateTime.Now.Month,
                Year = DateTime.Now.Year
            };
            var violaters = await _lateInEarlyOutService.GetLateInEarlyOutViolateOnMonth(criteria).ConfigureAwait(false);

            if (violaters.Any())
            {
                var isChange = await _hangFireService.ObjectIsChangedOrNotExists(LateInEarlyOutViolateOnMonth, violaters).ConfigureAwait(false);

                if (isChange)
                {
                    result.Code = 1;
                    await SendMessageViolateOnMonthToEmployees(violaters.Where(v => !string.IsNullOrWhiteSpace(v.ConversationId))).ConfigureAwait(false);

                    await SendMessageViolateOnMonthToRegisterConversation(violaters.Where(v => v.TimeOnMonth >= 4)).ConfigureAwait(false);

                    result = await _hangFireService.UpdateJsonObject(LateInEarlyOutViolateOnMonth, violaters).ConfigureAwait(false);
                }
            }

            return result;
        }

        public async Task<CommandResult> SendMessageViolateDisciplineAsync()
        {
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            var fromDate = toDate.AddYears(-1).AddDays(1);

            var criteria = new GetViolateLateInEarlyOutDisciplineCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            var violaters = await _lateInEarlyOutService.GetLateInEarlyOutDiscipline(criteria).ConfigureAwait(false);

            if (violaters.Any())
            {
                bool isChangedObject = await _hangFireService.ObjectIsChangedOrNotExists(ViolateLateInEarlyOutDiscipline, violaters).ConfigureAwait(false);

                if (isChangedObject)
                {
                    await SendMessageViolateDisciplineToGroup(violaters, fromDate, toDate).ConfigureAwait(false);

                    await _hangFireService.UpdateJsonObject(ViolateLateInEarlyOutDiscipline, violaters).ConfigureAwait(false);
                }
            }

            return new CommandResult();
        }

        private async Task SendMessageViolateOnMonthToEmployees(IEnumerable<LateInEarlyOutViolateModel> violaters)
        {
            if (violaters.Any())
            {
                string jsonObject = await _hangFireService.GetJsonObject(LateInEarlyOutViolateOnMonth).ConfigureAwait(false);
                await SendMessageViolateOnMonthAfterChecking(jsonObject, violaters).ConfigureAwait(false);
            }
        }

        private async Task SendMessageViolateOnMonthAfterChecking(string jsonObject, IEnumerable<LateInEarlyOutViolateModel> violaters)
        {
            if (!string.IsNullOrWhiteSpace(jsonObject))
            {
                IEnumerable<LateInEarlyOutViolateModel> oldViolaters = JsonConvert.DeserializeObject<IEnumerable<LateInEarlyOutViolateModel>>(jsonObject);
                foreach (var violater in violaters)
                {
                    if (oldViolaters.Any(o => o.EmpId == violater.EmpId && violater.TimeOnMonth != o.TimeOnMonth))
                    {
                        string message = GeneratePersonalMessageLateInEarlyOutViolateOnMonth(violater);
                        await _nexbotService.SendMessageAsync(violater.ConversationId, message).ConfigureAwait(false);
                    }
                }
            }
            else
            {
                foreach (var violater in violaters)
                {
                    string message = GeneratePersonalMessageLateInEarlyOutViolateOnMonth(violater);
                    await _nexbotService.SendMessageAsync(violater.ConversationId, message).ConfigureAwait(false);
                }
            }
        }

        private static string GeneratePersonalMessageLateInEarlyOutViolateOnMonth(LateInEarlyOutViolateModel violater)
        {
            var msg = string.Format(Messages.DearSkypeMessage, violater.NickName, Constants.NewLine);
            if (violater.TimeOnMonth >= 4)
            {
                msg = msg + string.Format(
                    Messages.EmployeeLateInEarlyOutViolateOnMonth,
                    violater.TimeOnMonth,
                    DateTime.Now.ToString(FormatShortDate),
                    Constants.NewLine);
            }
            else
            {
                msg += string.Format(
                    Messages.EmployeeLateInEarlyOutViolateOnMonthOnceMoreTime,
                    violater.TimeOnMonth,
                    DateTime.Now.ToString(FormatShortDate),
                    Constants.NewLine);
            }

            return msg;
        }

        private async Task SendMessageViolateOnMonthToRegisterConversation(IEnumerable<LateInEarlyOutViolateModel> violaters)
        {
            var conversations = await _nexbotService.GetRegisteredConversation(LateInEarlyOutViolateOnMonth).ConfigureAwait(false);
            if (conversations.Any() && violaters.Any())
            {
                string message = GenerateViolateOnMonthGroupMessage(violaters);
                foreach (var conversation in conversations)
                {
                    await _nexbotService.SendMessageAsync(conversation, message).ConfigureAwait(false);
                }
            }
        }

        private static string GenerateViolateOnMonthGroupMessage(IEnumerable<LateInEarlyOutViolateModel> violaters)
        {
            StringBuilder message = new StringBuilder();
            message.Append(string.Format(
                Messages.ThereIsNumberEmployeeViolateLateInEarlyOutOnMonth,
                violaters.Count(), DateTime.Now.ToString(FormatShortDate),
                Constants.NewLine));

            foreach (var violater in violaters)
            {
                message.Append(string.Format(
                    Messages.EmployeeViolateTime,
                    violater.FullName,
                    violater.NickName,
                    violater.TimeOnMonth,
                    Constants.NewLine));
            }

            message.Append(string.Format(
                Messages.AccessLinkToViewViolateLateInEarlyOutOnMonth,
                BaseUrl));

            return message.ToString();
        }

        private async Task SendMessageViolateDisciplineToGroup(IEnumerable<LateInEarlyOutDiscipline> violaters, DateTime fromDate, DateTime toDate)
        {
            var conversations = await _nexbotService.GetRegisteredConversation(ViolateLateInEarlyOutDiscipline).ConfigureAwait(false);

            if (conversations.Any())
            {
                string message = GenerateViolateDisciplineGroupMessage(violaters, fromDate, toDate);

                foreach (var conversation in conversations)
                {
                    await _nexbotService.SendMessageAsync(conversation, message).ConfigureAwait(false);
                }
            }
        }

        private static string GenerateViolateDisciplineGroupMessage(IEnumerable<LateInEarlyOutDiscipline> violaters, DateTime fromDate, DateTime toDate)
        {
            StringBuilder message = new StringBuilder();
            message.Append(string.Format(
                Messages.ThereIsNumberEmployeeViolateDiscipline,
                violaters.Count(),
                fromDate.ToString(FormatLongDate),
                toDate.ToString(FormatLongDate),
                Constants.NewLine));

            foreach (var violater in violaters)
            {
                message.Append(string.Format(
                    Messages.EmployeeViolateTime,
                    violater.FullName,
                    violater.NickName,
                    violater.TotalTimes,
                    Constants.NewLine));
            }

            message.Append(string.Format(
                Messages.AccessLinkToViewViolateDiscipline,
                BaseUrl));

            return message.ToString();
        }
    }
}