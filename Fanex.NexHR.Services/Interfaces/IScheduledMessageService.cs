namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IScheduledMessageService
    {
        Task<IEnumerable<Conversation>> GetConversationsAsync(int employeeId);

        Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(int year, int createdBy);

        Task<ScheduledMessage> GetScheduledMessageAsync(int scheduledMessageId);

        Task<CommandResult> SendScheduledMessages();
        Task SendScheduledVegetarianMessages();
        Task<IEnumerable<Conversation>> GetConversationsByCreatorAsync(int createdBy);

        Task<Conversation> GetConversationByIdAsync(int id);

    }
}