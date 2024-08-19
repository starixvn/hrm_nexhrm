namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models.ScheduledMessage;
    using Fanex.NexHR.Repositories.Interfaces;

    public class ScheduledMessageRepository : BaseRepository, IScheduledMessageRepository
    {
        public async Task<IEnumerable<Conversation>> GetConversationsByCreatorAsync(GetConversationsByCreatorCriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multiReader = await db.QueryMultipleAsync(criteria))
                {
                    var conversations = multiReader.Read<Conversation>();
                    var permittedEmployees = multiReader.Read<PermittedEmployee>();

                    foreach (var conversation in conversations)
                    {
                        conversation.PermittedEmployees = permittedEmployees.Where(x => x.ConversationId == conversation.ConversationId);
                    }

                    return conversations;
                }
            }
        }

        public async Task<Conversation> GetConversationByIdAsync(GetConversationsByIdCriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multiReader = await db.QueryMultipleAsync(criteria))
                {
                    var conversation = multiReader.Read<Conversation>().FirstOrDefault();
                    var permittedEmployees = multiReader.Read<PermittedEmployee>();
                    if (conversation != null)
                    {
                        conversation.PermittedEmployees = permittedEmployees.Where(x => x.ConversationId == conversation.ConversationId);
                    }

                    return conversation;
                }
            }
        }
    }
}