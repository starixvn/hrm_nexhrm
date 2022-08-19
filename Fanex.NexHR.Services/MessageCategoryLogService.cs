namespace Fanex.NexHR.Services
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Hangfire;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Newtonsoft.Json;

    public class MessageCategoryLogService : IMessageCategoryLogService
    {
        private readonly IBaseService baseService;

        public MessageCategoryLogService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<bool> ObjectIsChangedOrNotExists(MessageCategory categoryId, object value)
        {
            string jsonObject = JsonConvert.SerializeObject(value);

            var criteria = new CheckIfObjectNotChangeCriteria
            {
                CategoryId = categoryId,
                JsonObject = jsonObject
            };
            return !await baseService.GetDataEntityAsync<bool>(criteria).ConfigureAwait(false);
        }

        public async Task<string> GetJsonObject(MessageCategory categoryId)
        {
            var criteria = new GetJsonObjectByCategoryMessageCriteria
            {
                CategoryId = categoryId
            };

            return await baseService.GetDataEntityAsync<string>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> UpdateJsonObject(MessageCategory categoryId, object value)
        {
            string jsonObject = JsonConvert.SerializeObject(value);
            var criteria = new UpdateJsonObjectCriteria
            {
                CategoryId = categoryId,
                JsonObject = jsonObject
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}