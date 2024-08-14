namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models.ScheduledMessage;

    public interface IScheduledMessageRepository
    {
        Task<IEnumerable<Conversation>> GetConversationsByCreatorAsync(GetConversationsByCreatorCriteria criteria);

        Task<Conversation> GetConversationByIdAsync(GetConversationsByIdCriteria criteria);
    }
}