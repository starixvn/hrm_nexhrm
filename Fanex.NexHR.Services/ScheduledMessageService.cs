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
    using Microsoft.AspNetCore.Http.HttpResults;
    using NCrontab;
    using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ScheduledMessageService : IScheduledMessageService
    {
        private readonly IScheduledMessageRepository scheduledMessageRepository;
        private readonly INexbotService nexbotService;
        private readonly IHolidayService holidayService;
        private readonly IBaseService baseService;
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
            IBaseService baseService,
            AppSetting appSetting)
        {
            this.scheduledMessageRepository = scheduledMessageRepository;
            this.holidayService = holidayService;
            this.nexbotService = nexbotService;
            this.baseService = baseService;
            this.appSetting = appSetting;
        }

        public async Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(int year, int createdBy)
        {
            var criteria = new GetScheduleMessagesCriteria
            {
                Year = year,
                CreatedBy = createdBy
            };
            var result = await baseService.GetDataAsync<ScheduledMessage>(criteria);
            return result;
        }

        public async Task<ScheduledMessage> GetScheduledMessageAsync(int scheduledMessageId)
        {
            var criteria = new GetScheduleMessageCriteria
            {
                ScheduledMessageId = scheduledMessageId
            };
            var result = await baseService.GetDataEntityAsync<ScheduledMessage>(criteria);
            return result;
        }

        public async Task<CommandResult> SendScheduledMessages()
        {
            var currentDate = DateTimeOffset.Now.DateTime;
            currentDate = currentDate.AddSeconds(-currentDate.Second).AddMilliseconds(-currentDate.Millisecond);
            var redundantTick = currentDate.Ticks % 10000;
            currentDate = currentDate.AddTicks(-redundantTick);

            var criteria = new GetScheduledMessagesToProcessCriteria
            {
                FromDate = currentDate.AddMinutes(-4),
                ToDate = currentDate
            };

            var scheduledMessages = await baseService.GetDataAsync<ScheduledMessage>(criteria);
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
            var updateSentCriteria = new UpdateSentScheduledMessageStatusCriteria
            {
                ScheduledMessageIds = string.Join(",", sentMessageIds)
            };
            var result = await baseService.ExecuteNonQueryAsync(updateSentCriteria);

            return result;
        }

        public async Task<IEnumerable<Conversation>> GetConversationsAsync(int employeeId)
        {
            var criteria = new GetConversationsCriteria { EmployeeId = employeeId };
            var conversations = await baseService.GetDataAsync<Conversation>(criteria);
            return conversations;
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
            var criteria = new GetEmployeeConversationCriteria();
            var employeeConversations = await baseService.GetDataAsync<Employee>(criteria);
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