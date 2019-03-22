namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Models.ScheduledMessage;

    public interface IScheduledMessageService
    {
        Task<IEnumerable<GroupConversation>> GetGroupConversationsAsync();
    }
}