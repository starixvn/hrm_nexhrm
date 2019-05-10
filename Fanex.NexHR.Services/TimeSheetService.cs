namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Caching;
    using Fanex.NexHR.Criteria.TimeSheet;
    using Fanex.NexHR.Models.SkypeAccount;
    using Fanex.NexHR.Models.TimeSheet;
    using Fanex.NexHR.Services.Interfaces;

    public class TimeSheetService : BaseService, ITimeSheetService
    {
        private const string CacheKeyIgnoredReminder = "UserIgnoredReminderReachedWorkingTime";
 
        private readonly INexbotService nexbotService;
        

        public TimeSheetService()
            : this(null)
        {
        }

        public TimeSheetService(INexbotService nexbotService)
        {
            this.nexbotService = nexbotService ?? new NexbotService();
        }

        public void SendReminderReachedWorkingTime()
        {
            var criteria = new GetEmployeeReachedWorkingTimeCriteria();
            var workinghours = GetData<WorkingHour>(criteria);
            var ignoredUser = GetUserIgnoreReminderReachedWorkingTime();

            workinghours = workinghours.Where(t => !ignoredUser.Any(x => x.ConversationId == t.SkypeConversationId)).ToList();
            foreach (var workinghour in workinghours)
            {
                nexbotService.SendMessage(workinghour.SkypeConversationId, workinghour.MessageContent);
            }
        }

        public IList<ConversationUser> GetUserIgnoreReminderReachedWorkingTime()
        {
            var conversations = ConfiguredCacheService.Get<IList<ConversationUser>>(CacheKeyIgnoredReminder);
            if (conversations == null || !conversations.Any())
            {
                conversations = nexbotService.GetIgnoredSkypeAccountByMessageCategory(1);

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