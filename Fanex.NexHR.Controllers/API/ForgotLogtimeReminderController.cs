namespace Fanex.NexHR.Controllers.API
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.RedmineAPI;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.API;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;

    [Authorize]
    [JwtAuthentication]
    public class ForgotLogtimeReminderController : ApiController
    {
        private readonly IRedmineService redmineService;
        private readonly INexbotService nexbotService;

        public ForgotLogtimeReminderController()
        {
            redmineService = new RedmineService();
            nexbotService = new NexbotService();
        }

        [HttpPost]
        public async Task<IHttpActionResult> ForgotLogtime()
        {
            var forgotLogtimeEmployees = await redmineService.GetForgotLogtimeEmployees().ConfigureAwait(false);

            if (forgotLogtimeEmployees.Any())
            {
                SendMessageToEmployees(forgotLogtimeEmployees.Where(e => !string.IsNullOrWhiteSpace(e.ConversationId)));
                await SendMessageToGroups(forgotLogtimeEmployees, MessageCategory.ForgotSpendTime).ConfigureAwait(false);
            }

            return Ok();
        }

        private async Task SendMessageToGroups(IEnumerable<RedmineUser> forgotLogtimeEmployees, MessageCategory categoryId)
        {
            var conversationIds = await nexbotService.GetRegisteredConversation(categoryId).ConfigureAwait(false);

            if (conversationIds.Any())
            {
                string message = GenerateGroupMessage(forgotLogtimeEmployees);

                foreach (var conversationId in conversationIds)
                {
                    nexbotService.SendMessage(conversationId, message);
                }
            }
        }

        private static string GenerateGroupMessage(IEnumerable<RedmineUser> employees)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"Hi All. There is/are {employees.Count()} employee(s) forgot log time on Nexdev: {Constants.NewLine}");

            foreach (var employee in employees)
            {
                foreach (var date in employee.ForgotLogtimeDates)
                {
                    message.Append($"_- {employee.FullName} ({employee.NickName}) forgot log time on {date.ToString("dd MMM, yyyy")}_ {Constants.NewLine}");
                }
            }
            message.Append("Please notice that. Thanks ");

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
            message.Append($"Dear {employee.NickName}, {Constants.NewLine}");
            message.Append($"You forgot log time on: {Constants.NewLine}");
            foreach (var date in employee.ForgotLogtimeDates)
            {
                message.Append($"_- {date.ToString("dd MMM, yyyy")}_{Constants.NewLine}");
            }
            message.Append($"Please log time on https://nexdev.net/ or you can log time directly on https://hrm.nexcel.vn/ Dashboard. Thanks");

            return message.ToString();
        }
    }
}
