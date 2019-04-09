namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;

    public interface IScheduledMessageService
    {
        Task<IEnumerable<Conversation>> GetConversationsAsync(int employeeId);

        Task<CommandResult> CreateAsync(CreateScheduledMessage model);

        Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(int year);

        Task<ScheduledMessage> GetScheduledMessageAsync(int scheduledMessageId);

        Task<CommandResult> EditAsync(EditScheduledMessage model);

        Task<CommandResult> RemoveAsync(int scheduledMessageId);

        Task<CommandResult> SendScheduledMessages();

        Task<CommandResult> CreateConversationAsync(CreateConversation model);
    }
}