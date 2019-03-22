namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.ScheduledMessage;
    using Fanex.NexHR.Repositories.Interfaces;

    public class ScheduledMessageRepository : IScheduledMessageRepository
    {
        public async Task<IEnumerable<GroupConversation>> GetGroupConversationsAsync(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<GroupConversation>(criteria.GetSpParams()).ConfigureAwait(false);
            }
        }
    }
}