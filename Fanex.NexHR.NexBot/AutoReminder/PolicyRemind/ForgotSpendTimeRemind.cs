namespace Fanex.NexHR.NexBot.AutoReminder.PolicyRemind
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Criteria.Hangfire;
    using Hangfire;
    using Interfaces;
    using Models.Hangfire;
    using Services;
    using Services.Interfaces;

    public class ForgotSpendTimeRemind : INexbotEvent
    {
        private readonly BaseService baseService;
        private readonly IRedmineService redmineService;
        private readonly INexbotService nexbotService;
        private readonly string CronExpression = ConfigurationManager.AppSettings["MissingCheckInOutCronExpression"] ?? "0 8-18 * * 1-5";

        public ForgotSpendTimeRemind()
        {
            baseService = new BaseService();
            redmineService = new RedmineService();
            nexbotService = new NexbotService();
        }

        public void Initialize()
        {
            RecurringJob.AddOrUpdate(HangfireCategoryMessageType.ForgotSpendTime.GetDisplayName(), () => SendMessageToConversation(HangfireCategoryMessageType.ForgotSpendTime), CronExpression);
        }

        public async Task SendMessageToConversation(HangfireCategoryMessageType categoryId)
        {
            var employees = await baseService.GetDataAsync<EmployeeConversation>(new GetEmployeeToRemindForgotSpendTimeCriteria());

            if (employees.Any())
            {
                foreach (var employee in employees)
                {
                    await SendMessageToEmployee(employee);
                }
            }
        }

        private async Task SendMessageToEmployee(EmployeeConversation employee)
        {
            var forgotLogtimeDates = await redmineService.GetHaveNotLogTimeDates(employee.DomainUsername, employee.EmpId);
            if (forgotLogtimeDates.Any())
            {
                var message = GenerateMessage(employee, forgotLogtimeDates);
                await nexbotService.SendMessage(employee.ConversationId, message);
            }
        }

        private string GenerateMessage(EmployeeConversation employee, IList<DateTime> forgotDates)
        {
            var dates = string.Join(", ", forgotDates.Select(d => d.ToString("dd, MMM, yyyy")));
            var message = $"Dear {employee.NickName}, {Constants.NewLine}";
            message += $"You have forgot log time on _{dates}_. {Constants.NewLine}";
            message += $"Please log time on https://nexdev.net/ or you can log time directly on https://hrm.nexcel.vn/ Dashboard. Thanks";
            return message;
        }
    }
}