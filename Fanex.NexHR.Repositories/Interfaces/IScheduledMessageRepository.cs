namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IScheduledMessageRepository
    {
        Task<IEnumerable<Conversation>> GetConversationsAsync(GetConversationsCriteria criteria);
        Task<IEnumerable<Employee>> GetEmployeeConversationAsync(GetEmployeeConversationCriteria criteria);
        Task<IEnumerable<Conversation>> GetEmployeeConversationByIdAsync(GetEmployeeConversationByIdCriteria criteria);

        Task<CommandResult> CreateAsync(CreateScheduledMessageCriteria criteria);

        Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(GetScheduleMessagesCriteria criteria);

        Task<ScheduledMessage> GetScheduledMessageAsync(GetScheduleMessageCriteria criteria);

        Task<CommandResult> EditAsync(EditScheduledMessageCriteria criteria);

        Task<CommandResult> RemoveAsync(RemoveScheduleMessageCriteria criteria);

        Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesToProcessAsync(GetScheduledMessagesToProcessCriteria criteria);

        Task<CommandResult> UpdateSentMessageStatus(UpdateSentScheduledMessageStatusCriteria criteria);

        Task<CommandResult> CreateConversationAsync(CreateConversationCriteria criteria);

        Task<IEnumerable<Conversation>> GetConversationsByCreatorAsync(GetConversationsByCreatorCriteria criteria);

        Task<Conversation> GetConversationByIdAsync(GetConversationsByIdCriteria criteria);

        Task<CommandResult> EditConversationAsync(EditConversationCriteria criteria);
        Task<IEnumerable<ScheduledMessage>> GetScheduledVegetarianMessagesAsync(GetScheduledVegetarianMessageCriteria criteria);
        Task<IEnumerable<Conversation>> GetTeamHrConversationsAsync(GetTeamHrConversationCriteria criteria);
    }
}