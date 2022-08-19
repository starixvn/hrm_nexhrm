namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.TimeSheet;
    using Fanex.NexHR.Models.SkypeAccount;
    using Fanex.NexHR.Models.TimeSheet;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class TimeSheetService : ITimeSheetService
    {
        private const string CacheKeyIgnoredReminder = "UserIgnoredReminderReachedWorkingTime";

        private readonly INexbotService nexbotService;
        private readonly IBaseRepository respository;
        private readonly IBaseService baseService;

        public TimeSheetService(INexbotService nexbotService, IBaseRepository respository, IBaseService baseService)
        {
            this.nexbotService = nexbotService;
            this.respository = respository;
            this.baseService = baseService;
        }

        public async Task SendReminderReachedWorkingTime()
        {
            var criteria = new GetEmployeeReachedWorkingTimeCriteria();
            var workinghours = await baseService.GetDataAsync<WorkingHour>(criteria);
            var today = DateTime.Now.Date;

            if (workinghours != null && workinghours.Any())
            {
                var ignoredUser = await GetUserIgnoreReminderReachedWorkingTime();
                workinghours = workinghours.Where(t => !ignoredUser.Any(x => x.ConversationId == t.SkypeConversationId)).ToList();

                if (workinghours.Any())
                {
                    foreach (var workinghour in workinghours)
                    {
                        await nexbotService.SendMessageAsync(workinghour.SkypeConversationId, workinghour.MessageContent);
                    }

                    var employeeIds = string.Join(",", workinghours.Select(t => t.EmployeeId));
                    await WriteLogForSentSuccessful(employeeIds, today);
                }
            }
        }

        private async Task WriteLogForSentSuccessful(string employeeIds, DateTime workDate)
        {
            var criteria = new InsertLogReachedWorkingTimeCriteria
            {
                EmployeeIds = employeeIds,
                WorkDate = workDate
            };

            await baseService.ExecuteNonQueryAsync(criteria);
        }

        private async Task<IList<ConversationUser>> GetUserIgnoreReminderReachedWorkingTime()
        {
            var conversations = ConfiguredCacheService.Get<IList<ConversationUser>>(CacheKeyIgnoredReminder);
            if (conversations == null || !conversations.Any())
            {
                conversations = await nexbotService.GetIgnoredSkypeAccountByMessageCategoryAsync(MessageCategory.ReachedWorkingTime);

                if (conversations != null && conversations.Any())
                {
                    ConfiguredCacheService.Set(CacheKeyIgnoredReminder, conversations,
                        new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1)));
                }
            }

            return conversations;
        }
    }
}