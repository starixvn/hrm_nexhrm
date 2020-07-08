namespace Fanex.NexHR.Services.AutoJobService
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Interfaces;

    public class BotMissingCheckInCheckOutReminderSerivce : IBotMissingCheckInCheckOutReminderSerivce
    {
        private const int MissingCheckInCheckOutCategoryId = 6;
        private const string FormatDate = "dd MMM, yyyy";
        private const string CheckIn = "Check In";
        private const string CheckOut = "Check Out";
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        private readonly IEmployeeService _employeeService;
        private readonly INexbotService _nexbotService;
        private readonly CommandResult Result = new CommandResult { Code = 0 };

        public BotMissingCheckInCheckOutReminderSerivce() : this(null, null)
        {
        }

        public BotMissingCheckInCheckOutReminderSerivce(
            IEmployeeService employeeService,
            INexbotService nexbotService
        )
        {
            _employeeService = employeeService ?? new EmployeeService();
            _nexbotService = nexbotService ?? new NexbotService();
        }

        public async Task<CommandResult> SendMessageAsync()
        {
            var missingCheckInOuts = await _employeeService.GetMissingCheckInOutEmployeeAsync().ConfigureAwait(false);
            if (missingCheckInOuts.Any())
            {
                await SendMessageToGroups(MissingCheckInCheckOutCategoryId, missingCheckInOuts).ConfigureAwait(false);
                await SendMessageToEmployees(missingCheckInOuts.Where(e => !string.IsNullOrWhiteSpace(e.ConversationId))).ConfigureAwait(false);
            }

            return Result;
        }

        private async Task SendMessageToGroups(int categoryId, IEnumerable<MissingCheckInOutEmployee> missingCheckInOuts)
        {
            var conversationIds = await _nexbotService.GetRegisteredConversation(categoryId).ConfigureAwait(false);
            if (conversationIds.Any())
            {
                string msg = GenerateGroupMessage(missingCheckInOuts);

                foreach (var conversationId in conversationIds)
                {
                    var result = await _nexbotService.SendMessageAsync(conversationId, msg).ConfigureAwait(false);
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
                missingCheckInOuts.Count() > 1 ? "are" : "is",
                missingCheckInOuts.Count(),
                missingCheckInOuts.Count() > 1 ? "s" : string.Empty,
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
                Messages.AccessLinkToViewMore,
                $"{BaseUrl}/report/attendance"));

            return message.ToString();
        }

        private async Task SendMessageToEmployees(IEnumerable<MissingCheckInOutEmployee> employees)
        {
            foreach (var employee in employees)
            {
                var message = GenerateMessageForEmployee(employee);
                var result = await _nexbotService.SendMessageAsync(employee.ConversationId, message).ConfigureAwait(false);
                if (!result)
                {
                    Result.Code = 0;
                }
            }
        }

        private static string GenerateMessageForEmployee(MissingCheckInOutEmployee employee)
        {
            var message = $"Dear {employee.NickName}, {Constants.NewLine}";
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