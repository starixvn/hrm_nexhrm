namespace Fanex.NexHR.Services.AutoJobService
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.RedmineAPI;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FogotLogtimeReminderService : IBotFogotLogtimeReminderService
    {
        private const string formatDate = "dd MMM, yyyy";
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        private static readonly string Redmine_API_Address = ConfigurationManager.AppSettings["Redmine_API_Address"];
        private readonly IRedmineService redmineService;
        private readonly INexbotService nexbotService;

        public FogotLogtimeReminderService()
        {
            redmineService = new RedmineService();
            nexbotService = new NexbotService();
        }

        public async Task<CommandResult> SendMessageForgotLogtimeAsync()
        {
            var result = new CommandResult { Code = 0 };
            var forgotLogtimeEmployees = await redmineService.GetForgotLogtimeEmployees().ConfigureAwait(false);

            if (forgotLogtimeEmployees.Any())
            {
                result.Code = 1;
                SendMessageToEmployees(forgotLogtimeEmployees.Where(e => !string.IsNullOrWhiteSpace(e.ConversationId)));
                await SendMessageToGroups(forgotLogtimeEmployees, MessageCategory.ForgotSpendTime).ConfigureAwait(false);
            }

            return result;
        }

        private async Task SendMessageToGroups(IEnumerable<RedmineUser> forgotLogtimeEmployees, MessageCategory categoryId)
        {
            var conversationIds = await nexbotService.GetRegisteredConversation(categoryId).ConfigureAwait(false);

            if (conversationIds.Any())
            {
                string message = GenerateGroupMessage(forgotLogtimeEmployees);

                foreach (var conversationId in conversationIds)
                {
                    await nexbotService.SendMessageAsync(conversationId, message).ConfigureAwait(false);
                }
            }
        }

        private static string GenerateGroupMessage(IEnumerable<RedmineUser> employees)
        {
            StringBuilder message = new StringBuilder();
            message.Append(string.Format(Messages.AnounceFogotLogtime, employees.Count(), Constants.NewLine));

            foreach (var employee in employees)
            {
                foreach (var date in employee.ForgotLogtimeDates)
                {
                    message.Append(string.Format(Messages.EmployeeFogotLogtime, employee.FullName, employee.NickName, date.ToString(formatDate), Constants.NewLine));
                }
            }
            message.Append(string.Format(Messages.ThankNotice));

            return message.ToString();
        }

        private void SendMessageToEmployees(IEnumerable<RedmineUser> forgotLogtimeEmployees)
        {
            foreach (var employee in forgotLogtimeEmployees)
            {
                var message = GenerateMessage(employee);
                nexbotService.SendMessage(employee.ConversationId, message);
            }
        }

        private static string GenerateMessage(RedmineUser employee)
        {
            StringBuilder message = new StringBuilder();
            message.Append(string.Format(Messages.DearSkypeMessage, employee.NickName, Constants.NewLine));
            message.Append(string.Format(Messages.FogotLogtime, Constants.NewLine));
            foreach (var date in employee.ForgotLogtimeDates)
            {
                message.Append($"_- {date.ToString(formatDate)}_{Constants.NewLine}");
            }
            message.Append(string.Format(Messages.RemindLogtime, Redmine_API_Address, BaseUrl));

            return message.ToString();
        }
    }
}
