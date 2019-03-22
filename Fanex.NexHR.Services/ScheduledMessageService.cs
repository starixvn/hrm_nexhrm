namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models.ScheduledMessage;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class ScheduledMessageService : IScheduledMessageService
    {
        private readonly IScheduledMessageRepository scheduledMessageRepository;

        public ScheduledMessageService()
        {
            scheduledMessageRepository = new ScheduledMessageRepository();
        }

        public async Task<IEnumerable<GroupConversation>> GetGroupConversationsAsync()
        {
            return await scheduledMessageRepository.GetGroupConversationsAsync(new GetGroupConversationsCriteria()).ConfigureAwait(false);
        }
    }
}