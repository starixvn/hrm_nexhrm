namespace Fanex.NexHR.Services.AutoJobService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.MailDeliver.MailClient;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.LateInEarlyOut;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Core;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Routing.Template;
    using Newtonsoft.Json;
    using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

    public class BotLateInEarlyOutViolateOnMonthWarningSerivce : IBotLateInLateOutAutoJobService
    {
        private const int ViolatedTime = 4;
        private const string FormatShortDate = "MMM, yyyy";
        private const string FormatLongDate = "dd MMM, yyyy";
        private readonly EmailSettings emailSettings;
        private readonly string BaseUrl;
        private readonly IMessageCategoryLogService messageCategoryLog;
        private readonly INexbotService nexbotService;
        private readonly ILateInEarlyOutService lateInEarlyOutService;
        private readonly IScheduledMessageRepository scheduledMessageRepository;
        private readonly IEmailService emailService;
        private readonly AppSetting appSetting;
        private readonly IWebHostEnvironment environment;
        private readonly IBaseService baseService;

        public BotLateInEarlyOutViolateOnMonthWarningSerivce(
            IMessageCategoryLogService messageCategoryLog,
            INexbotService nexbotService,
            ILateInEarlyOutService lateInEarlyOutService,
            IScheduledMessageRepository scheduledMessageRepository,
            IBaseService baseService,
            IEmailService emailService,
            EmailSettings emailSettings,
            IWebHostEnvironment environment,
            AppSetting appSetting
        )
        {
            this.messageCategoryLog = messageCategoryLog;
            this.nexbotService = nexbotService;
            this.lateInEarlyOutService = lateInEarlyOutService;
            this.scheduledMessageRepository = scheduledMessageRepository;
            this.baseService = baseService;
            this.emailService = emailService;
            this.appSetting = appSetting;
            this.environment = environment;
            this.emailSettings = emailSettings;
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
                    result = await SendMessageDeductToConversation(conversations.Select(m => m.ConversationId), message).ConfigureAwait(false);
                }
            }

            return result;
        }

        public async Task<CommandResult> DeductAsync()
        {
            var result = new CommandResult { Code = 0 };
            var CreatedByID = 279;
            var criteria = new GetLateInEarlyOutDeductionCriteria { Year = 2022 };
            var deductedEmployees = await lateInEarlyOutService.GetLateInEarlyOutDeduction(criteria).ConfigureAwait(false);
            
            foreach (var item in deductedEmployees)
            {
                var insertLeaveDeductionCriteria = new InsertLeaveDeductionCriteria
                {
                    EmployeeId = item.EmpId,
                    Description = Messages.DescriptionAutoDeduct,
                    DeductDate = DateTime.UtcNow,
                    IsAnnual = true,
                    TotalDays = Convert.ToDecimal(0.5),
                    CreatedBy = CreatedByID
                };
                result = await baseService.ExecuteNonQueryAsync(insertLeaveDeductionCriteria);

                result = await SendMessageDeduct(item);
                SendMailDeductToEmp(item);
            }
            return result;
        }

        private async Task<CommandResult> SendMessageDeduct(LateInEarlyOutModel item)
        {
            var conversations = await scheduledMessageRepository.GetConversationsAsync(new GetConversationsCriteria { EmployeeId = item.EmpId }).ConfigureAwait(false);
            var result = new CommandResult { Code = 0 };
            if (conversations.Any())
            {
                string message = string.Format(Messages.MessageAutoDeduct, item.NickName);
                result = await SendMessageDeductToConversation(conversations.Where(x => x.ConversationName.ToLower().Equals(item.NickName.ToLower()))
                    .Select(m => m.ConversationId.ToString()), message).ConfigureAwait(false);
            }
            return result;
        }

        private void SendMailDeductToEmp(LateInEarlyOutModel item)
        {
            var EmailTemplatePath = "EmailTemplates/AutoJobDeduct.Request.txt";
            string templatePath = Path.Combine(environment.ContentRootPath, EmailTemplatePath);
            string body = string.Format(
                   Utilities.ReadTextFile(templatePath),
                   item.NickName,
                   appSetting.BaseUrl,
                   emailSettings.CompanyMailSign);

            emailService.SendEmail(
                emailSettings.Email,
                string.Join(";", item.Email),
                string.Empty,
                Messages.SubjectEmailAutoDeduct,
                body);
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

        private async Task<CommandResult> SendMessageDeductToConversation(IEnumerable<string> conversations, string message)
        {
            var result = false;
            foreach (var conversation in conversations)
            {
                result = await nexbotService.SendMessageAsync(conversation, message).ConfigureAwait(false);
            }

            return new CommandResult { Code = result ? 0 : 1 };
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

        private string GenerateMessageAutoDeduct(IEnumerable<LateInEarlyOutModel> deductedEmployees)
        {
            StringBuilder message = new StringBuilder();
            message.Append(string.Format(
                Messages.MessageAutoDeduct,
                deductedEmployees.Count()
                ));

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