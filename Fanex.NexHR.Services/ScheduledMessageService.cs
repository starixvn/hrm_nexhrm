namespace Fanex.NexHR.Services
{
    using Fanex.Caching;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Holiday;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.ScheduledMessage;
    using Fanex.NexHR.Models.SkypeAccount;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using NCrontab;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ScheduledMessageService : IScheduledMessageService
    {
        private readonly IScheduledMessageRepository scheduledMessageRepository;
        private readonly INexbotService nexbotService;
        private readonly IHolidayService holidayService;
        private readonly AppSetting appSetting;
        private const string FirstLunar = "01";
        private const int FifteenLunar = 15;
        private const int IdEmpCreateVegetarianMessages = 125;
        private const int NumMonthOfScheduleVegetarianMessages = 2;
        private const int itemEachBatch = 10;
        private const string CacheKeyIgnoredReminder = "UserIgnoredReminderVegetarianRestration";

        public ScheduledMessageService(IScheduledMessageRepository scheduledMessageRepository,
            IHolidayService holidayService,
            INexbotService nexbotService,
            AppSetting appSetting)
        {
            this.scheduledMessageRepository = scheduledMessageRepository;
            this.holidayService = holidayService;
            this.nexbotService = nexbotService;
            this.appSetting = appSetting;
        }

        public async Task<CommandResult> CreateAsync(CreateScheduledMessage model)
        {
            return await scheduledMessageRepository.CreateAsync(new CreateScheduledMessageCriteria
            {
                ConversationId = model.ConversationId,
                SendDate = model.SendDate,
                Content = model.Content,
                CreatedBy = model.CreatedBy,
                CronExpression = model.CronExpression,
                StepNumber = model.StepNumber,
                RepeatTypeId = model.RepeatTypeId
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(int year, int createdBy)
        {
            return await scheduledMessageRepository.GetScheduledMessagesAsync(new GetScheduleMessagesCriteria
            {
                Year = year,
                CreatedBy = createdBy
            }).ConfigureAwait(false);
        }

        public async Task<ScheduledMessage> GetScheduledMessageAsync(int scheduledMessageId)
        {
            return await scheduledMessageRepository.GetScheduledMessageAsync(new GetScheduleMessageCriteria
            {
                ScheduledMessageId = scheduledMessageId
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditAsync(EditScheduledMessage model)
        {
            return await scheduledMessageRepository.EditAsync(new EditScheduledMessageCriteria
            {
                ScheduledMessageId = model.ScheduledMessageId,
                ConversationId = model.ConversationId,
                SendDate = model.SendDate,
                Content = model.Content,
                CronExpression = model.CronExpression,
                StepNumber = model.StepNumber,
                RepeatTypeId = model.RepeatTypeId
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> RemoveAsync(int scheduledMessageId)
        {
            return await scheduledMessageRepository.RemoveAsync(new RemoveScheduleMessageCriteria
            {
                ScheduledMessageId = scheduledMessageId
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> SendScheduledMessages()
        {
            var currentDate = DateTimeOffset.Now.DateTime;
            currentDate = currentDate.AddSeconds(-currentDate.Second).AddMilliseconds(-currentDate.Millisecond);
            var redundantTick = currentDate.Ticks % 10000;
            currentDate = currentDate.AddTicks(-redundantTick);

            var scheduledMessages = await scheduledMessageRepository.GetScheduledMessagesToProcessAsync(new GetScheduledMessagesToProcessCriteria
            {
                FromDate = currentDate.AddMinutes(-4),
                ToDate = currentDate
            }).ConfigureAwait(false);

            var sentMessageIds = new List<int>();

            foreach (var scheduledMessage in scheduledMessages)
            {
                if (scheduledMessage.CronExpression != null)
                {
                    CrontabSchedule schedule = CrontabSchedule.Parse(scheduledMessage.CronExpression);
                    var nextRun = schedule.GetNextOccurrence(DateTimeOffset.Now.DateTime.AddMinutes(-1));
                    if (nextRun == currentDate)
                    {
                        await nexbotService.SendMessageAsync(scheduledMessage.SkypeConversationId, scheduledMessage.Content);
                    }
                }
                else
                {
                    if (await nexbotService.SendMessageAsync(scheduledMessage.SkypeConversationId, scheduledMessage.Content))
                    {
                        sentMessageIds.Add(scheduledMessage.ScheduledMessageId);
                    }
                }
            }

            var result = await scheduledMessageRepository.UpdateSentMessageStatus(new UpdateSentScheduledMessageStatusCriteria
            {
                ScheduledMessageIds = string.Join(",", sentMessageIds)
            }).ConfigureAwait(false);

            return result;
        }

        public async Task<IEnumerable<Conversation>> GetConversationsAsync(int employeeId)
        {
            return await scheduledMessageRepository.GetConversationsAsync(new GetConversationsCriteria { EmployeeId = employeeId }).ConfigureAwait(false);
        }

        public async Task<CommandResult> CreateConversationAsync(CreateConversation model)
        {
            return await scheduledMessageRepository.CreateConversationAsync(new CreateConversationCriteria
            {
                SkypeConversationId = model.SkypeConversationId,
                ConversationName = model.ConversationName,
                PermittedEmployeeIds = model.PermittedEmployeeIds,
                CreatedBy = model.CreatedBy
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Conversation>> GetConversationsByCreatorAsync(int createdBy)
        {
            return await scheduledMessageRepository.GetConversationsByCreatorAsync(new GetConversationsByCreatorCriteria
            {
                CreatedBy = createdBy
            }).ConfigureAwait(false);
        }

        public async Task<Conversation> GetConversationByIdAsync(int id)
        {
            return await scheduledMessageRepository.GetConversationByIdAsync(new GetConversationsByIdCriteria
            {
                ConversationId = id
            }).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditConversationAsync(EditConversation model)
        {
            return await scheduledMessageRepository.EditConversationAsync(new EditConversationCriteria
            {
                ConversationId = model.ConversationId,
                SkypeConversationId = model.SkypeConversationId,
                ConversationName = model.ConversationName,
                PermittedEmployeeIds = model.PermittedEmployeeIds,
                EditedBy = model.EditedBy,
                IsActive = model.IsActive
            }).ConfigureAwait(false);
        }

        public async Task SendScheduledVegetarianMessages()
        {
            var workingSchedules = await holidayService.GetWorkDateEmployeeCalendar(new GetWorkDateEmployeeCalendarCriteria
            {
                EmpId = IdEmpCreateVegetarianMessages,
                Year = DateTimeOffset.Now.Year,
            });
            var scheduledVegetarianList = GetFirstAndFifteenLunerOfMonths(new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime
                                                .AddMonths(NumMonthOfScheduleVegetarianMessages).AddDays(-1), workingSchedules);
            foreach (var item in scheduledVegetarianList)
            {
                var sendDate = GetSendDate(item, workingSchedules);
                if (sendDate == DateTimeOffset.Now.Date)
                {
                    var dateContent = new DateTimeOffset(item.Solar.SolarYear, item.Solar.SolarMonth, item.Solar.SolarDay, 0, 0, 0, TimeSpan.Zero).DateTime;
                    var messageContent = string.Format(Labels.VegetarianMessageContent, ((int)dateContent.DayOfWeek + 1),
                                            dateContent.ToDateMonthYearFormatedDate(), item.Lunar.LunarDay == 1 ? FirstLunar : item.Lunar.LunarDay);
                    string message = string.Format(Labels.VegetarianMessages, messageContent);
                    await SendReminderVegetarianRestrationToAllEmp(message, messageContent, dateContent);
                }
            }
        }

        private async Task SendReminderVegetarianRestrationToAllEmp(string message, string messageContent, DateTime onDate)
        {
            var employeeConversations = await scheduledMessageRepository.GetEmployeeConversationAsync(new GetEmployeeConversationCriteria()).ConfigureAwait(false);
            var ignoredUser = await GetUserIgnoreVegetarianRestration();
            
            employeeConversations = employeeConversations.Where(t => !ignoredUser.Any(x => x.ConversationId == t.ConversationId)).ToList();
            
            if (employeeConversations.Any())
            {
                var batches = new List<List<Employee>>();
                
                int totalBatch = (int)Math.Ceiling((double)employeeConversations.Count() / itemEachBatch);
                for (int i = 0; i < totalBatch; i++)
                {
                    batches.Add(employeeConversations.Skip(i * itemEachBatch).Take(itemEachBatch).ToList());
                }

                foreach (var batch in batches)
                {
                    var tasks = new List<Task>();
                    foreach (var employeeConversation in batch)
                    {
                        tasks.Add(nexbotService.SendRegistVegetarianMealAsync(employeeConversation.ConversationId, 
                            employeeConversation.Id, appSetting.FBCompanyId, onDate, message, messageContent));
                    }

                    await Task.WhenAll(tasks);
                }
            }
        }

        private static DateTime GetSendDate(SolarLunarViewModel solarLunarViewModel, IEnumerable<WorkingSchedule> workingSchedules)
        {
            var dateSend = new DateTimeOffset(solarLunarViewModel.Solar.SolarYear, solarLunarViewModel.Solar.SolarMonth, solarLunarViewModel.Solar.SolarDay, 0, 0, 0, TimeSpan.Zero).DateTime;
            do
            {
                dateSend = dateSend.AddDays(-1);
            }
            while (IsHolidayOrDayOff(workingSchedules, dateSend));
            return dateSend;
        }

        private static List<SolarLunarViewModel> GetFirstAndFifteenLunerOfMonths(DateTime dateTime, IEnumerable<WorkingSchedule> workingSchedules)
        {
            List<SolarLunarViewModel> solarLunarViewModels = new();
            DateTime currentDate = DateTimeOffset.Now.DateTime;
            do
            {
                Lunar lunar = LunarSolarConverter.SolarToLunar(new Solar
                {
                    SolarDay = currentDate.Day,
                    SolarMonth = currentDate.Month,
                    SolarYear = currentDate.Year
                });
                var dateCheck = new DateTimeOffset(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0, TimeSpan.Zero).DateTime;
                if (!IsHolidayOrDayOff(workingSchedules, dateCheck) && IsFirstAndFifteenLuner(lunar))
                {
                    solarLunarViewModels.Add(new SolarLunarViewModel
                    {
                        Solar = new Solar
                        {
                            SolarDay = currentDate.Day,
                            SolarMonth = currentDate.Month,
                            SolarYear = currentDate.Year
                        },
                        Lunar = lunar,
                    });
                }
                currentDate = currentDate.AddDays(1);
            }
            while (currentDate <= dateTime);
            return solarLunarViewModels;
        }

        private async Task<IList<ConversationUser>> GetUserIgnoreVegetarianRestration()
        {
            var conversations = ConfiguredCacheService.Get<IList<ConversationUser>>(CacheKeyIgnoredReminder);
            if (conversations == null || !conversations.Any())
            {
                conversations = await nexbotService.GetIgnoredSkypeAccountByMessageCategoryAsync(MessageCategory.VegetarianRestration);

                if (conversations != null && conversations.Any())
                {
                    ConfiguredCacheService.Set(CacheKeyIgnoredReminder, conversations,
                        new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1)));
                }
            }

            return conversations;
        }

        private static bool IsHolidayOrDayOff(IEnumerable<WorkingSchedule> workingSchedules, DateTime dateTime) => workingSchedules.Any(x => dateTime == x.WorkDate);
        private static bool IsFirstAndFifteenLuner(Lunar lunar) => lunar.LunarDay == 1 || lunar.LunarDay == FifteenLunar;
    }
}