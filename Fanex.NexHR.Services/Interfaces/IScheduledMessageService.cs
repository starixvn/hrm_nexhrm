namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IScheduledMessageService
    {
        Task<IEnumerable<Conversation>> GetConversationsAsync(int employeeId);

        Task<CommandResult> CreateAsync(CreateScheduledMessage model);

        Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(int year, int createdBy);

        Task<ScheduledMessage> GetScheduledMessageAsync(int scheduledMessageId);

        Task<CommandResult> EditAsync(EditScheduledMessage model);

        Task<CommandResult> RemoveAsync(int scheduledMessageId);

        Task<CommandResult> SendScheduledMessages();
        Task SendScheduledVegetarianMessages();

        Task<CommandResult> CreateConversationAsync(CreateConversation model);

        Task<IEnumerable<Conversation>> GetConversationsByCreatorAsync(int createdBy);

        Task<Conversation> GetConversationByIdAsync(int id);

        Task<CommandResult> EditConversationAsync(EditConversation model);
    }
}