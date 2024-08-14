namespace Fanex.NexHR.Services.AutoJobService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Interfaces;

    public class BotMissingCheckInCheckOutReminderSerivce : IBotAutoJobService
    {
        private const string FormatDate = "dd MMM, yyyy";
        private const string CheckIn = "Check In";
        private const string CheckOut = "Check Out";
        private readonly string BaseUrl;
        private readonly IEmployeeService employeeService;
        private readonly INexbotService nexbotService;
        private readonly CommandResult Result = new CommandResult { Code = 0 };

        public BotMissingCheckInCheckOutReminderSerivce(
            IEmployeeService employeeService,
            INexbotService nexbotService,
            AppSetting appSetting
        )
        {
            this.employeeService = employeeService;
            this.nexbotService = nexbotService;
            BaseUrl = appSetting.BaseUrl;
        }

        public async Task<CommandResult> SendMessageAsync()
        {
            var missingCheckInOuts = await employeeService.GetMissingCheckInOutEmployeeAsync().ConfigureAwait(false);
            if (missingCheckInOuts.Any())
            {
                await SendMessageToGroups(MessageCategory.MissingCheckInCheckOut, missingCheckInOuts).ConfigureAwait(false);
                await SendMessageToEmployees(missingCheckInOuts.Where(e => !string.IsNullOrWhiteSpace(e.ConversationId))).ConfigureAwait(false);
            }

            return Result;
        }

        private async Task SendMessageToGroups(MessageCategory categoryId, IEnumerable<MissingCheckInOutEmployee> missingCheckInOuts)
        {
            var conversations = await nexbotService.GetRegisteredConversationAsync(categoryId);
            if (conversations.Any())
            {
                string msg = GenerateGroupMessage(missingCheckInOuts);

                foreach (var conversation in conversations)
                {
                    var result = await nexbotService.SendMessageAsync(conversation.ConversationId, msg).ConfigureAwait(false);
                    if (!result)
                    {
                        Result.Code = 0;
                    }
                }
            }
        }

        private string GenerateGroupMessage(IEnumerable<MissingCheckInOutEmployee> missingCheckInOuts)
        {
            StringBuilder message = new StringBuilder();
            message.Append(string.Format(
                Messages.AnounceNumberEmployeeMissCheckInOut,
                missingCheckInOuts.Count(),
                Constants.NewLine));

            foreach (var employee in missingCheckInOuts)
            {
                var missType = employee.Type == 1 ? CheckIn : CheckOut;
                message.Append(string.Format(
                    Messages.EmployeeMissingCheckInOut,
                    employee.FullName,
                    employee.NickName,
                    missType,
                    employee.AttendanceDate.ToString(FormatDate),
                    Constants.NewLine));
            }

            message.Append(string.Format(
                Messages.AccessLinkToViewMoreCheckInCheckOut,
                BaseUrl));

            return message.ToString();
        }

        private async Task SendMessageToEmployees(IEnumerable<MissingCheckInOutEmployee> employees)
        {
            foreach (var employee in employees)
            {
                await nexbotService.SendRemindCheckInCheckOutAsync(employee);
            }
        }
    }
}