namespace Fanex.NexHR.Services.AutoJobService
{
    using System.Collections.Generic;
    using System.Configuration;
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
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        private readonly IEmployeeService employeeService;
        private readonly INexbotService nexbotService;
        private readonly CommandResult Result = new CommandResult { Code = 0 };

        public BotMissingCheckInCheckOutReminderSerivce() : this(null, null)
        {
        }

        public BotMissingCheckInCheckOutReminderSerivce(
            IEmployeeService employeeService,
            INexbotService nexbotService
        )
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.nexbotService = nexbotService ?? new NexbotService();
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
            var conversations = nexbotService.GetRegisteredConversation(categoryId);
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

        private static string GenerateGroupMessage(IEnumerable<MissingCheckInOutEmployee> missingCheckInOuts)
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
                var message = GenerateMessageForEmployee(employee);
                var result = await nexbotService.SendMessageAsync(employee.ConversationId, message).ConfigureAwait(false);
                if (!result)
                {
                    Result.Code = 0;
                }
            }
        }

        private static string GenerateMessageForEmployee(MissingCheckInOutEmployee employee)
        {
            var message = string.Format(Messages.DearSkypeMessage, employee.NickName, Constants.NewLine);
            message += string.Format(
                Messages.YouForgotCheckInOut,
                employee.Type == 1 ? CheckIn : CheckOut,
                employee.AttendanceDate.ToString(FormatDate),
                employee.ShiftCode,
                Constants.NewLine);

            message += string.Format(
                Messages.ReviewAndMakeCheckInOutRequestAtLink,
                BaseUrl,
                employee.AttendanceDate.ToString("MM/dd/yyyy"),
                employee.Type);

            return message;
        }
    }
}