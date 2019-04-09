namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;

    public interface IScheduledMessageRepository
    {
        Task<IEnumerable<Conversation>> GetConversationsAsync(GetConversationsCriteria criteria);

        Task<CommandResult> CreateAsync(CreateScheduledMessageCriteria criteria);

        Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(GetScheduleMessagesCriteria criteria);

        Task<ScheduledMessage> GetScheduledMessageAsync(GetScheduleMessageCriteria criteria);

        Task<CommandResult> EditAsync(EditScheduledMessageCriteria criteria);

        Task<CommandResult> RemoveAsync(RemoveScheduleMessageCriteria criteria);

        Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesToProcessAsync(GetScheduledMessagesToProcessCriteria criteria);

        Task<CommandResult> UpdateSentMessageStatus(UpdateSentScheduledMessageStatusCriteria criteria);

        Task<CommandResult> CreateConversationAsync(CreateConversationCriteria criteria);
    }
}