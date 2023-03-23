namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using NCrontab;
    using NPOI.SS.Formula.Functions;

    public class ScheduledMessageService : IScheduledMessageService
    {
        private readonly IScheduledMessageRepository scheduledMessageRepository;
        private readonly INexbotService nexbotService;

        public ScheduledMessageService(IScheduledMessageRepository scheduledMessageRepository, INexbotService nexbotService)
        {
            this.scheduledMessageRepository = scheduledMessageRepository;
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

        public async Task<CommandResult> SendScheduledVegetarianMessages()
        {
            var scheduledMessages = await GetScheduledMessagesAsync(DateTime.Now.Year, 536).ConfigureAwait(false);
            DateTime dateTimeNow = DateTime.Now;
            DateTime dateTimeNext = dateTimeNow.AddMonths(1);
            List<Solar> lunarListFirstMonth = HandleLunar(dateTimeNow);
            List<Solar> lunarListSecondMonth = HandleLunar(new DateTime(dateTimeNext.Year, dateTimeNext.Month, 1));
            List<CreateScheduledMessageCriteria> schedule = HandleCriteria(lunarListFirstMonth, scheduledMessages);
            schedule.AddRange(HandleCriteria(lunarListSecondMonth, scheduledMessages));
            var result = new CommandResult();
            foreach (var item in schedule)
            {
               result = await scheduledMessageRepository.CreateAsync(item).ConfigureAwait(false);
            }
            return result;
        }

        private static List<CreateScheduledMessageCriteria> HandleCriteria(List<Solar> solars, 
            IEnumerable<ScheduledMessage> scheduledMessages)
        {
            List<CreateScheduledMessageCriteria> schedule = new();
            foreach (var item in solars)
            {
                var firstLunar = "01";
                Lunar lunar = LunarSolarConverter.SolarToLunar(new Solar
                {
                    solarDay = item.solarDay,
                    solarMonth = item.solarMonth,
                    solarYear = item.solarYear
                });

                var dateContent = new DateTime(item.solarYear, item.solarMonth, item.solarDay);
                string content = String.Format(Labels.VegetarianMessages, ((int)dateContent.DayOfWeek + 1), 
                    dateContent.ToString("dd/MM/yyyy"), lunar.lunarDay == 1 ? firstLunar : lunar.lunarDay);

                if(!scheduledMessages.Any(x => x.Content == content)) {
                    var dateBefore = new DateTime(item.solarYear, item.solarMonth, item.solarDay).AddDays(-1);
                    var sendDate = GenerateSendDate(dateBefore);
                    schedule.Add(new CreateScheduledMessageCriteria
                    {
                        ConversationId = 1,
                        SendDate = new DateTime(sendDate.Year, sendDate.Month, sendDate.Day, 11, 0, 0),
                        Content = content,
                        CreatedBy = 536,
                        CronExpression = null,
                        RepeatTypeId = null
                    });
                }
            }
            return schedule;
        }

        private static DateTime GenerateSendDate(DateTime dateTime)
        {
            DateTime dateSend;
            int before = 0;
            do{
                dateSend = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(before--);
            }
            while (((int)dateSend.DayOfWeek + 1) == 7 || ((int)dateSend.DayOfWeek + 1) == 1);
            return dateSend;
        }

        private static List<Solar> HandleLunar(DateTime dateTime)
        {
            List<Solar> solars = new();
            for(int i = dateTime.Day; i <= DateInMount(dateTime.Month, dateTime.Year); i++)
            {
                Lunar lunar = LunarSolarConverter.SolarToLunar(new Solar
                {
                    solarDay = i,
                    solarMonth = dateTime.Month,
                    solarYear = dateTime.Year
                });
                var dateCheck = new DateTime(dateTime.Year, dateTime.Month, i);
                if ((lunar.lunarDay == 1 || lunar.lunarDay == 15) && (((int)dateCheck.DayOfWeek + 1) != 7 && ((int)dateCheck.DayOfWeek + 1) != 1))
                {
                    solars.Add(new Solar { 
                        solarDay = i,
                        solarMonth = dateTime.Month,
                        solarYear = dateTime.Year
                    });
                }
            }
            return solars;
        }

        private static int DateInMount(int th, int nm)
        {
            int songay = 0;
            if (th >= 1 && th <= 12)
            {
                switch (th)
                {
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                    case 12: songay = 31; break;
                    case 4:
                    case 6:
                    case 9:
                    case 11: songay = 30; break;

                    case 2:
                        if (nm % 400 == 0 || (nm % 4 == 0 && nm % 100 != 0))
                            songay = 29;
                        else
                            songay = 28;
                        break;
                }
            }
            return songay;
        }
    }
}