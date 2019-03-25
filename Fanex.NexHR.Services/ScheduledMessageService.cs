namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class ScheduledMessageService :  IScheduledMessageService
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

        public async Task<CommandResult> CreateAsync(CreateScheduledMessage model)
        {
            return await scheduledMessageRepository.CreateAsync(new CreateScheduledMessageCriteria
            {
                ConversationId = model.ConversationId,
                SendDate = model.SendDate,
                Content = model.Content,
                CreatedBy = model.CreatedBy
            }).ConfigureAwait(false);
        }
    }
}