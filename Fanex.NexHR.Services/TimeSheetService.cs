namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Caching;
    using Fanex.NexHR.Criteria.TimeSheet;
    using Fanex.NexHR.Models.SkypeAccount;
    using Fanex.NexHR.Models.TimeSheet;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class TimeSheetService : BaseService, ITimeSheetService
    {
        private const string CacheKeyIgnoredReminder = "UserIgnoredReminderReachedWorkingTime";
        private const int MessageCategory = 9;

        private readonly INexbotService nexbotService;
        private readonly IBaseRepository respository;

        public TimeSheetService()
            : this(null, null)
        {
        }

        public TimeSheetService(INexbotService nexbotService, IBaseRepository respository)
        {
            this.nexbotService = nexbotService ?? new NexbotService();
            this.respository = respository ?? new BaseRepository();
        }

        public void SendReminderReachedWorkingTime()
        {
            var criteria = new GetEmployeeReachedWorkingTimeCriteria();
            var workinghours = GetData<WorkingHour>(criteria);
            var today = DateTime.Now.Date;

            if(workinghours !=null && workinghours.Any())
            {
                var ignoredUser = GetUserIgnoreReminderReachedWorkingTime();
                workinghours = workinghours.Where(t => !ignoredUser.Any(x => x.ConversationId == t.SkypeConversationId)).ToList();

                if (workinghours.Any())
                {
                    foreach (var workinghour in workinghours)
                    {
                        nexbotService.SendMessage(workinghour.SkypeConversationId, workinghour.MessageContent);
                    }

                    var employeeIds = string.Join(",", workinghours.Select(t => t.EmployeeId));
                    WriteLogForSentSuccessful(employeeIds, today);
                }
            }
            
        }

        private void WriteLogForSentSuccessful(string employeeIds, DateTime workDate)
        {
            var criteria = new InsertLogReachedWorkingTimeCriteria
            {
                EmployeeIds = employeeIds,
                WorkDate = workDate
            };

            respository.Insert(criteria);
        }

        private IList<ConversationUser> GetUserIgnoreReminderReachedWorkingTime()
        {
            var conversations = ConfiguredCacheService.Get<IList<ConversationUser>>(CacheKeyIgnoredReminder);
            if (conversations == null || !conversations.Any())
            {
                conversations = nexbotService.GetIgnoredSkypeAccountByMessageCategory(MessageCategory);

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