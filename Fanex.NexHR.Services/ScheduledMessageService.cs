namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.Holiday;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Holiday;
    using Fanex.NexHR.Models.ScheduledMessage;
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
        private const string FirstLunar = "01";
        private const int FifteenLunar = 15;
        private const int HourSendVegetarianMessages = 11;
        private const int IdEmpCreateVegetarianMessages = 536;
        private const int NumMonthOfScheduleVegetarianMessages = 2;
        public ScheduledMessageService(IScheduledMessageRepository scheduledMessageRepository,
            IHolidayService holidayService,
            INexbotService nexbotService)
        {
            this.scheduledMessageRepository = scheduledMessageRepository;
            this.holidayService = holidayService;
            this.nexbotService = nexbotService;
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
            var currentDate = DateTime.Now;
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
                    var nextRun = schedule.GetNextOccurrence(DateTime.Now.AddMinutes(-1));
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

        public async Task<CommandResult> CreateScheduledVegetarianMessages()
        {
            var result = new CommandResult();
            var holidays = await holidayService.GetByYear(new GetHolidayByYearCriteria { Year = DateTime.Now.Year });
            var scheduledVegetarianMessages = await scheduledMessageRepository.GetScheduledVegetarianMessagesAsync(
                                                new GetScheduledVegetarianMessageCriteria { Year = DateTime.Now.Year });
            var scheduledVegetarianList = GetFirstAndFifteenLunerOfMonths(
                                                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(NumMonthOfScheduleVegetarianMessages).AddDays(-1), holidays);
            foreach (var item in scheduledVegetarianList)
            {
                var dateContent = new DateTime(item.Solar.solarYear, item.Solar.solarMonth, item.Solar.solarDay);
                string content = String.Format(Labels.VegetarianMessages, ((int)dateContent.DayOfWeek + 1),
                                        dateContent.ToString("dd/MM/yyyy"), item.Lunar.lunarDay == 1 ? FirstLunar : item.Lunar.lunarDay);
                if (!scheduledVegetarianMessages.Any(x => x.Content == content))
                {
                    var criteria = new CreateScheduledMessageCriteria
                    {
                        ConversationId = 1,
                        SendDate = GetSendDate(item, holidays),
                        Content = content,
                        CreatedBy = IdEmpCreateVegetarianMessages,
                        CronExpression = null,
                        RepeatTypeId = null
                    };
                    result = await scheduledMessageRepository.CreateAsync(criteria).ConfigureAwait(false);
                }
            }
            return result;
        }

        private static DateTime GetSendDate(SolarLunarViewModel solarLunarViewModel, IEnumerable<Holiday> holidays)
        {
            var dateSend = new DateTime(solarLunarViewModel.Solar.solarYear, solarLunarViewModel.Solar.solarMonth, solarLunarViewModel.Solar.solarDay);
            do
            {
                dateSend = dateSend.AddDays(-1);
            }
            while (IsDayOff(dateSend)
            || IsHoliday(holidays, dateSend));
            return dateSend.AddHours(HourSendVegetarianMessages);
        }

        private static List<SolarLunarViewModel> GetFirstAndFifteenLunerOfMonths(DateTime dateTime, IEnumerable<Holiday> holidays)
        {
            List<SolarLunarViewModel> solarLunarViewModels = new();
            DateTime currentDate = DateTime.Now;
            do
            {
                Lunar lunar = LunarSolarConverter.SolarToLunar(new Solar
                {
                    solarDay = currentDate.Day,
                    solarMonth = currentDate.Month,
                    solarYear = currentDate.Year
                });
                var dateCheck = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                if (!IsDayOff(dateCheck) && !IsHoliday(holidays, dateCheck) && IsFirstAndFifteenLuner(lunar))
                {
                    solarLunarViewModels.Add(new SolarLunarViewModel
                    {
                        Solar = new Solar
                        {
                            solarDay = currentDate.Day,
                            solarMonth = currentDate.Month,
                            solarYear = currentDate.Year
                        },
                        Lunar = lunar,
                    });
                }
                currentDate = currentDate.AddDays(1);
            }
            while (currentDate <= dateTime);
            return solarLunarViewModels;
        }

        private static bool IsDayOff(DateTime dateTime) => (int)dateTime.DayOfWeek == (int)DayOfWeek.Saturday || (int)dateTime.DayOfWeek == (int)DayOfWeek.Sunday;
        private static bool IsHoliday(IEnumerable<Holiday> holidays, DateTime dateTime) => holidays.Any(x => dateTime >= x.FromDate && dateTime <= x.ToDate);
        private static bool IsFirstAndFifteenLuner(Lunar lunar) => lunar.lunarDay == 1 || lunar.lunarDay == FifteenLunar;
    }
}