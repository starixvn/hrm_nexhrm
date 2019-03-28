namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;
    using Fanex.NexHR.Repositories.Interfaces;

    public class ScheduledMessageRepository : BaseRepository, IScheduledMessageRepository
    {
        public async Task<IEnumerable<GroupConversation>> GetGroupConversationsAsync(GetGroupConversationsCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<GroupConversation>(criteria.GetSpParams()).ConfigureAwait(false);
            }
        }

        public async Task<CommandResult> CreateAsync(CreateScheduledMessageCriteria criteria)
        {
            return await ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(GetScheduleMessagesCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<ScheduledMessage>(criteria.GetSpParams()).ConfigureAwait(false);
            }
        }

        public async Task<ScheduledMessage> GetScheduledMessageAsync(GetScheduleMessageCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return await db.QueryEntityAsync<ScheduledMessage>(criteria.GetSpParams()).ConfigureAwait(false);
            }
        }

        public async Task<CommandResult> EditAsync(EditScheduledMessageCriteria criteria)
        {
            return await ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> RemoveAsync(RemoveScheduleMessageCriteria criteria)
        {
            return await ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}