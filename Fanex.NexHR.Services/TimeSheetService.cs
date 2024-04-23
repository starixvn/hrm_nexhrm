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
    using Fanex.NexHR.Services.Interfaces;

    public class TimeSheetService : ITimeSheetService
    {
        private const string CacheKeyIgnoredReminder = "UserIgnoredReminderReachedWorkingTime";
        private const string CacheKeyIgnoredCheckOutNotify = "UserIgnoredCheckOutNotify";
        private const int itemEachBatch = 10;

        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;

        public TimeSheetService(INexbotService nexbotService, IBaseService baseService)
        {
            this.nexbotService = nexbotService;
            this.baseService = baseService;
        }

        public async Task SendReminderReachedWorkingTime()
        {
            var criteria = new GetEmployeeReachedWorkingTimeCriteria();
            var workingHours = await baseService.GetDataAsync<WorkingHour>(criteria);
            var today = DateTimeOffset.Now.Date;

            if (workingHours != null && workingHours.Any())
            {
                var ignoredUser = await GetUserIgnoredMessageByCategoryAsync(MessageCategory.ReachedWorkingTime, CacheKeyIgnoredReminder);
                workingHours = workingHours.Where(t => !ignoredUser.Any(x => x.ConversationId == t.SkypeConversationId)).ToList();

                if (workingHours.Any())
                {
                    await SendMessage(workingHours);

                    var employeeIds = string.Join(",", workingHours.Select(t => t.EmployeeId));
                    await WriteLogForSentSuccessful(employeeIds, today, "ReachedWorkingHours");
                }
            }
        }

        private async Task SendMessage(IEnumerable<WorkingHour> workingHours)
        {
            var batches = new List<List<WorkingHour>>();

            int totalBatch = (int)Math.Ceiling((double)workingHours.Count() / itemEachBatch);
            for (int i = 0; i < totalBatch; i++)
            {
                batches.Add(workingHours.Skip(i * itemEachBatch).Take(itemEachBatch).ToList());
            }

            foreach (var batch in batches)
            {
                var tasks = new List<Task>();
                foreach (var workinghour in batch)
                {
                    tasks.Add(nexbotService.SendMessageAsync(workinghour.SkypeConversationId, workinghour.MessageContent));
                }

                await Task.WhenAll(tasks);
            }
        }

        private async Task WriteLogForSentSuccessful(string employeeIds, DateTime workDate, string insertType)
        {
            var criteria = new InsertLogReachedWorkingTimeCriteria
            {
                EmployeeIds = employeeIds,
                WorkDate = workDate,
                InsertedType = insertType
            };

            await baseService.ExecuteNonQueryAsync(criteria);
        }

        private async Task<IList<ConversationUser>> GetUserIgnoredMessageByCategoryAsync(MessageCategory category, string cacheKey)
        {
            var conversations = ConfiguredCacheService.Get<IList<ConversationUser>>(cacheKey);
            if (conversations == null || !conversations.Any())
            {
                conversations = await nexbotService.GetIgnoredSkypeAccountByMessageCategoryAsync(category);

                if (conversations != null && conversations.Any())
                {
                    ConfiguredCacheService.Set(cacheKey, conversations,
                        new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1)));
                }
            }

            return conversations;
        }

        public async Task ImportDataFromBioStar()
        {
            var criteria = new ImportDataFromBioStarCriteria();

            await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task NotifyEmployeeAfterCheckOut()
        {
            var currentDate = DateTimeOffset.Now.Date;

            var criteria = new GetEmployeeHadCheckOutCriteria();
            var checkoutEmployees = await baseService.GetDataAsync<CheckOutNotifyRecord>(criteria);

            if (checkoutEmployees != null && checkoutEmployees.Any())
            {
                var ignoredUser = await GetUserIgnoredMessageByCategoryAsync(MessageCategory.CheckOutNotification, CacheKeyIgnoredCheckOutNotify);
                checkoutEmployees = checkoutEmployees.Where(t => !ignoredUser.Any(x => x.ConversationId == t.ConversationId)).ToList();

                if (checkoutEmployees.Any())
                {
                    await DivideBatchAndSendMessageEmployeeCheckOut(checkoutEmployees);

                    var employeeIds = string.Join(",", checkoutEmployees.Select(t => t.EmployeeId));
                    await WriteLogForSentSuccessful(employeeIds, currentDate, "NotifyCheckOut");
                }
            }
        }

        private async Task DivideBatchAndSendMessageEmployeeCheckOut(IEnumerable<CheckOutNotifyRecord> checkoutRecords)
        {
            var batches = new List<List<CheckOutNotifyRecord>>();

            int totalBatch = (int)Math.Ceiling((double)checkoutRecords.Count() / itemEachBatch);
            for (int i = 0; i < totalBatch; i++)
            {
                batches.Add(checkoutRecords.Skip(i * itemEachBatch).Take(itemEachBatch).ToList());
            }

            foreach (var batch in batches)
            {
                var tasks = new List<Task>();
                foreach (var workinghour in batch)
                {
                    tasks.Add(nexbotService.SendMessageAsync(workinghour.ConversationId, workinghour.MessageContent));
                }

                await Task.WhenAll(tasks);
            }
        }
    }
}