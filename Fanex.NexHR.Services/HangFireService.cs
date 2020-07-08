namespace Fanex.NexHR.Services
{
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Fanex.NexHR.Criteria.Hangfire;
    using Fanex.NexHR.Services.Interfaces;

    public class HangFireService : BaseService, IHangFireService
    {
        public async Task<bool> ObjectIsChangedOrNotExists(int categoryId, object value)
        {
            string jsonObject = JsonConvert.SerializeObject(value);

            var criteria = new CheckIfObjectNotChangeCriteria
            {
                CategoryId = categoryId,
                JsonObject = jsonObject
            };
            return !await GetDataEntityAsync<bool>(criteria).ConfigureAwait(false);
        }

        public async Task<string> GetJsonObject(int categoryId)
        {
            var criteria = new GetJsonObjectByCategoryMessageCriteria
            {
                CategoryId = categoryId
            };

            return await GetDataEntityAsync<string>(criteria).ConfigureAwait(false);
        }

        public async Task UpdateJsonObject(int categoryId, object value)
        {
            string jsonObject = JsonConvert.SerializeObject(value);
            var criteria = new UpdateJsonObjectCriteria
            {
                CategoryId = categoryId,
                JsonObject = jsonObject
            };

            await ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}
