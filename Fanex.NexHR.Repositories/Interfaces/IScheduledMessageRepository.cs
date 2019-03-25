namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;

    public interface IScheduledMessageRepository
    {
        Task<IEnumerable<GroupConversation>> GetGroupConversationsAsync(GetGroupConversationsCriteria criteria);
        Task<CommandResult> CreateAsync(CreateScheduledMessageCriteria criteria);
    }
}