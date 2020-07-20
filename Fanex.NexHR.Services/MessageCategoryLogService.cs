namespace Fanex.NexHR.Services
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Hangfire;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Newtonsoft.Json;

    public class MessageCategoryLogService : BaseService, IMessageCategoryLogService
    {
        public async Task<bool> ObjectIsChangedOrNotExists(MessageCategory categoryId, object value)
        {
            string jsonObject = JsonConvert.SerializeObject(value);

            var criteria = new CheckIfObjectNotChangeCriteria
            {
                CategoryId = categoryId,
                JsonObject = jsonObject
            };
            return !await GetDataEntityAsync<bool>(criteria).ConfigureAwait(false);
        }

        public async Task<string> GetJsonObject(MessageCategory categoryId)
        {
            var criteria = new GetJsonObjectByCategoryMessageCriteria
            {
                CategoryId = categoryId
            };

            return await GetDataEntityAsync<string>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> UpdateJsonObject(MessageCategory categoryId, object value)
        {
            string jsonObject = JsonConvert.SerializeObject(value);
            var criteria = new UpdateJsonObjectCriteria
            {
                CategoryId = categoryId,
                JsonObject = jsonObject
            };

            return await ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}