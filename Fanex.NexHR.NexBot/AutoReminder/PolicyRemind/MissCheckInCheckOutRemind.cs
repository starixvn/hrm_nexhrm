namespace Fanex.NexHR.WebSite.AutoReminder.PolicyRemind
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Hangfire;
    using Hangfire;
    using NexBot.Interfaces;
    using NexHR.Models.Hangfire;
    using Services;
    using Services.Interfaces;
    using ViewModels;

    public class MissCheckInCheckOutRemind : INexbotEvent
    {
        private readonly IEmployeeService employeeService;
        private readonly INexbotService nexbotService;
        private readonly BaseService baseService;

        // Every 30 mins from 8am to 18pm, Mon to Fri
        private readonly string CronExpression = ConfigurationManager.AppSettings["MissingCheckInOutCronExpression"] ?? "0 8-18 * * 1-5";

        public MissCheckInCheckOutRemind()
        {
            employeeService = new EmployeeService();
            baseService = new BaseService();
            nexbotService = new NexbotService();
        }

        public void Initialize()
        {
            RecurringJob.AddOrUpdate(HangfireCategoryMessageType.MissingCheckInOut.GetDisplayName(), () => SendMessageToConversation(HangfireCategoryMessageType.MissingCheckInOut), CronExpression);
        }

        public async Task SendMessageToConversation(HangfireCategoryMessageType categoryId)
        {
            var missCheckInOut = await employeeService.GetMissingCheckInOutAsync(new GetMissingCheckInOutEmployeesCriteria()).ConfigureAwait(false);

            if (missCheckInOut.Any())
            {
                var employeeCoversations = await GetAllEmployeeConversationIds(categoryId).ConfigureAwait(false);

                if (employeeCoversations.Any())
                {
                    await SendMessageToEachOne(employeeCoversations, missCheckInOut).ConfigureAwait(false);
                }
            }
        }

        private async Task SendMessageToEachOne(IEnumerable<EmployeeConversation> conversations, IEnumerable<MissingCheckInOutModel> missCheckInOut)
        {
            foreach (var conversation in conversations)
            {
                if (missCheckInOut.Any(e => e.NickName.Equals(conversation.NickName)))
                {
                    var message = GenerateMessage(missCheckInOut.FirstOrDefault(e => e.NickName.Equals(conversation.NickName)));
                    await nexbotService.SendMessage(conversation.ConversationId, message).ConfigureAwait(false);
                }
            }
        }

        private static string GenerateMessage(MissingCheckInOutModel model)
        {
            var hrmLink = "https://hrm.nexcel.vn/checkinout/Create";
            var missType = model.MissingType == 1 ? "Check In" : "Check Out";
            var message = $"Dear {model.NickName}, {Constants.NewLine}";
            message += $"You have forgot _{missType}_ on _{model.AttendanceDate.ToString("dd, MMM, yyyy")}_ (shift {model.ShiftCode}). {Constants.NewLine}";
            message += $"Please make a request at: {hrmLink}";
            return message;
        }

        private async Task<IEnumerable<EmployeeConversation>> GetAllEmployeeConversationIds(HangfireCategoryMessageType categoryId)
        {
            var criteria = new GetAllEmployeeConversationCriteria { CategoryId = categoryId };
            return await baseService.GetDataAsync<EmployeeConversation>(criteria).ConfigureAwait(false);
        }
    }
}