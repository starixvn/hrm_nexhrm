namespace Fanex.NexHR.Services.AutoJobService
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Fanex.NexHR.Resources;
    using Interfaces;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BotLateInEarlyOutViolateOnMonthWarningSerivce : IBotLateInLateOutAutoJobService
    {
        private readonly string BaseUrl;
        private const int ViolatedTime = 4;
        private const int IdEmpDeductLateInEarlyOut = 279;
        private const string FormatShortDate = "MMM, yyyy";
        private const string FormatLongDate = "dd MMM, yyyy";
        private readonly IMessageCategoryLogService messageCategoryLog;
        private readonly ILogServiceProvider logService;
        private readonly INexbotService nexbotService;
        private readonly ILateInEarlyOutService lateInEarlyOutService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;

        public BotLateInEarlyOutViolateOnMonthWarningSerivce(
            IMessageCategoryLogService messageCategoryLog,
            INexbotService nexbotService,
            IRequestService requestService,
            ILateInEarlyOutService lateInEarlyOutService,
            IBaseService baseService,
            ILogServiceProvider logService,
            AppSetting appSetting
        )
        {
            this.messageCategoryLog = messageCategoryLog;
            this.nexbotService = nexbotService;
            this.requestService = requestService;
            this.lateInEarlyOutService = lateInEarlyOutService;
            this.baseService = baseService;
            this.logService = logService;
            BaseUrl = appSetting.BaseUrl;
        }

        public async Task<CommandResult> SendMessageViolateOnMonthAsync()
        {
            var result = new CommandResult { Code = 0 };
            var criteria = new GetLateInEarlyOutViolaterOnCurrentMonthCriteria
            {
                Month = DateTime.Now.Day == 1 ? (DateTime.Now.Month - 1) : DateTime.Now.Month,
                Year = DateTime.Now.Year
            };

            var violaters = await lateInEarlyOutService.GetLateInEarlyOutViolateOnMonth(criteria).ConfigureAwait(false);
            if (violaters.Any())
            {
                var isChange = await messageCategoryLog.ObjectIsChangedOrNotExists(MessageCategory.LateInEarlyOutViolateInMonth, violaters).ConfigureAwait(false);

                if (isChange)
                {
                    result.Code = 1;
                    await SendMessageViolateOnMonthToEmployees(violaters.Where(v => !string.IsNullOrWhiteSpace(v.ConversationId))).ConfigureAwait(false);
                    await SendMessageViolateOnMonthToRegisterConversation(violaters.Where(v => v.TimeOnMonth >= ViolatedTime)).ConfigureAwait(false);
                    result = await messageCategoryLog.UpdateJsonObject(MessageCategory.LateInEarlyOutViolateInMonth, violaters).ConfigureAwait(false);
                }
            }

            return result;
        }

        public async Task<CommandResult> SendMessageViolateDisciplineAsync()
        {
            var result = new CommandResult { Code = 0 };
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            var fromDate = toDate.AddYears(-1).AddDays(1);

            var criteria = new GetViolateLateInEarlyOutDisciplineCriteria
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            var violaters = await lateInEarlyOutService.GetLateInEarlyOutDiscipline(criteria).ConfigureAwait(false);

            if (violaters.Any())
            {
                bool isChangedObject = await messageCategoryLog.ObjectIsChangedOrNotExists(MessageCategory.ViolateLateInEarlyOutDiscipline, violaters).ConfigureAwait(false);
                if (isChangedObject)
                {
                    result.Code = 1;
                    await SendMessageViolateDisciplineToGroup(violaters, fromDate, toDate).ConfigureAwait(false);
                    result = await messageCategoryLog.UpdateJsonObject(MessageCategory.ViolateLateInEarlyOutDiscipline, violaters).ConfigureAwait(false);
                }
            }

            return result;
        }

        public async Task<CommandResult> SendMessageDeductAsync()
        {
            var result = new CommandResult { Code = 0 };
            var criteria = new GetLateInEarlyOutDeductionCriteria { Year = DateTime.Now.Year };
            var deductedEmployees = await lateInEarlyOutService.GetLateInEarlyOutDeduction(criteria).ConfigureAwait(false);

            if (deductedEmployees.Any())
            {
                var conversations = await nexbotService.GetRegisteredConversationAsync(MessageCategory.DeductLateInEarlyOut);
                if (conversations.Any())
                {
                    var message = GenerateMessageDeduct(deductedEmployees);
                    result = await nexbotService.SendMessageDeductToConversation(conversations.Select(m => m.ConversationId), message).ConfigureAwait(false);
                }
            }

            return result;
        }

        public async Task<CommandResult> DeductAsync()
        {
            var result = new CommandResult { Code = 0 };
            var criteria = new GetLateInEarlyOutDeductionCriteria { Year = DateTime.Now.Year };
            var deductedEmployees = await lateInEarlyOutService.GetLateInEarlyOutDeduction(criteria).ConfigureAwait(false);

            foreach (var item in deductedEmployees)
            {
                var insertLeaveDeductionCriteria = new InsertLeaveDeductionCriteria
                {
                    EmployeeId = item.EmpId,
                    Description = Messages.DescriptionAutoDeductLateInEarlyOut,
                    DeductDate = DateTime.Now,
                    IsAnnual = true,
                    TotalDays = Convert.ToDecimal(0.5),
                    CreatedBy = IdEmpDeductLateInEarlyOut
                };
                result = await baseService.ExecuteNonQueryAsync(insertLeaveDeductionCriteria);

                if (result.IsSuccessful)
                {
                    try
                    {
                        _ = await nexbotService.SendMessageDeductLateInEarlyOutToEmp(item);
                        requestService.SendMailDeductLateInEarlyOutToEmp(item);
                    }
                    catch (Exception ex)
                    {
                        logService.LogException(ex);
                    }
                }
            }

            if (deductedEmployees.Any() && result.IsSuccessful)
            {
                _ = await nexbotService.SendMessageDeductLateInEarlyOutToHrTeam(deductedEmployees);
            }

            return result;
        }

        public async Task<CommandResult> SendMessageReminderLateInEarlyOut()
        {
            var result = new CommandResult { Code = 0 };
            var reminderLateInEarlyOuts = await lateInEarlyOutService.GetAllReminderLateInEarlyOut(new GetAllReminderLateInEarlyOut()).ConfigureAwait(false);
            if (reminderLateInEarlyOuts.Any())
            {
                _ = await nexbotService.SendMessageReminderLateInEarlyOutToEmp(reminderLateInEarlyOuts);
                _ = await nexbotService.SendMessageReminderLateInEarlyOutToHr(reminderLateInEarlyOuts);
            }
            return result;
        }

        private async Task SendMessageViolateOnMonthToEmployees(IEnumerable<LateInEarlyOutViolateModel> violaters)
        {
            if (violaters.Any())
            {
                string jsonObject = await messageCategoryLog.GetJsonObject(MessageCategory.LateInEarlyOutViolateInMonth).ConfigureAwait(false);
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
                        await nexbotService.SendMessageAsync(violater.ConversationId, message).ConfigureAwait(false);
                    }
                }
            }
            else
            {
                foreach (var violater in violaters)
                {
                    string message = GeneratePersonalMessageLateInEarlyOutViolateOnMonth(violater);
                    await nexbotService.SendMessageAsync(violater.ConversationId, message).ConfigureAwait(false);
                }
            }
        }

        private static string GeneratePersonalMessageLateInEarlyOutViolateOnMonth(LateInEarlyOutViolateModel violater)
        {
            var msg = string.Format(Messages.DearSkypeMessage, violater.NickName, Constants.NewLine);
            if (violater.TimeOnMonth >= ViolatedTime)
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
            var conversations = await nexbotService.GetRegisteredConversationAsync(MessageCategory.LateInEarlyOutViolateInMonth);
            if (conversations.Any() && violaters.Any())
            {
                string message = GenerateViolateOnMonthGroupMessage(violaters);
                foreach (var conversation in conversations)
                {
                    await nexbotService.SendMessageAsync(conversation.ConversationId, message).ConfigureAwait(false);
                }
            }
        }

        private string GenerateViolateOnMonthGroupMessage(IEnumerable<LateInEarlyOutViolateModel> violaters)
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
            var conversations = await nexbotService.GetRegisteredConversationAsync(MessageCategory.ViolateLateInEarlyOutDiscipline);

            if (conversations.Any())
            {
                string message = GenerateViolateDisciplineGroupMessage(violaters, fromDate, toDate);

                foreach (var conversation in conversations)
                {
                    await nexbotService.SendMessageAsync(conversation.ConversationId, message).ConfigureAwait(false);
                }
            }
        }

        private string GenerateViolateDisciplineGroupMessage(IEnumerable<LateInEarlyOutDiscipline> violaters, DateTime fromDate, DateTime toDate)
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



        private string GenerateMessageDeduct(IEnumerable<LateInEarlyOutModel> deductedEmployees)
        {
            StringBuilder message = new StringBuilder();
            message.Append(string.Format(
                Messages.ThereIsNumberEmployeeNeedToDeductForLateInEarlyOut,
                deductedEmployees.Count(),
                Constants.NewLine));

            foreach (var e in deductedEmployees)
            {
                string balanceCurrentYear = (e.BalanceCurrentYear / 60).ToString("0.0");
                message.Append(string.Format(
                    Messages.BalanceCurrentYearHour,
                    e.FullName,
                    e.NickName,
                    balanceCurrentYear,
                    Constants.NewLine));
            }

            message.Append(string.Format(
                Messages.AccessLinkAndReviewToTakeDeduction,
                BaseUrl));

            return message.ToString();
        }
    }
}