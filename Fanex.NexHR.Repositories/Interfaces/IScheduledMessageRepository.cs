namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.ScheduledMessage;

    public interface IScheduledMessageRepository
    {
        Task<IEnumerable<GroupConversation>> GetGroupConversationsAsync(ICriteria criteria);
    }
}