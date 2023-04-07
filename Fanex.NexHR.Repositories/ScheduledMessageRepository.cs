namespace Fanex.NexHR.Repositories
{
    using Fanex.Data;
    using Fanex.NexHR.Criteria.ScheduledMessage;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduledMessage;
    using Fanex.NexHR.Repositories.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ScheduledMessageRepository : BaseRepository, IScheduledMessageRepository
    {
        public async Task<IEnumerable<Conversation>> GetConversationsAsync(GetConversationsCriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<Conversation>(criteria);
            }
        }

        public async Task<IEnumerable<Conversation>> GetEmployeeConversationAsync(GetEmployeeConversationCriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<Conversation>(criteria);
            }
        }

        public async Task<IEnumerable<Conversation>> GetEmployeeConversationByIdAsync(GetEmployeeConversationByIdCriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<Conversation>(criteria);
            }
        }

        public async Task<CommandResult> CreateAsync(CreateScheduledMessageCriteria criteria)
        {
            return await ExecuteNonQueryAsync(criteria);
        }

        public async Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync(GetScheduleMessagesCriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<ScheduledMessage>(criteria);
            }
        }

        public async Task<ScheduledMessage> GetScheduledMessageAsync(GetScheduleMessageCriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryEntityAsync<ScheduledMessage>(criteria);
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

        public async Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesToProcessAsync(GetScheduledMessagesToProcessCriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<ScheduledMessage>(criteria);
            }
        }

        public async Task<CommandResult> UpdateSentMessageStatus(UpdateSentScheduledMessageStatusCriteria criteria)
        {
            return await ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> CreateConversationAsync(CreateConversationCriteria criteria)
        {
            return await ExecuteNonQueryAsync(criteria);
        }

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

                    conversation.PermittedEmployees = permittedEmployees.Where(x => x.ConversationId == conversation.ConversationId);

                    return conversation;
                }
            }
        }

        public async Task<CommandResult> EditConversationAsync(EditConversationCriteria criteria)
        {
            return await ExecuteNonQueryAsync(criteria);
        }

        public async Task<IEnumerable<ScheduledMessage>> GetScheduledVegetarianMessagesAsync(GetScheduledVegetarianMessageCriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<ScheduledMessage>(criteria);
            }
        }

        public async Task<IEnumerable<Conversation>> GetTeamHrConversationsAsync(GetTeamHrConversationCriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<Conversation>(criteria);
            }
        }
    }
}