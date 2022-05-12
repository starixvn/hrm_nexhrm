namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using NCrontab;

    public class ScheduledMessageService : IScheduledMessageService
    {
        private readonly IScheduledMessageRepository scheduledMessageRepository;
        private readonly INexbotService nexbotService;

        public ScheduledMessageService()
        {
            scheduledMessageRepository = new ScheduledMessageRepository();
            nexbotService = new NexbotService();
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
                        nexbotService.SendMessage(scheduledMessage.SkypeConversationId, scheduledMessage.Content);
                    }
                }
                else
                {
                    if (nexbotService.SendMessage(scheduledMessage.SkypeConversationId, scheduledMessage.Content))
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
    }
}