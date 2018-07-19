namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Enums;
    using Criteria.Hangfire;
    using Interfaces;
    using Newtonsoft.Json;

    public class NexbotService : BaseService, INexbotService
    {
        private readonly Uri NexbotUrl = new Uri("https://t1.tkns.stream/api/CustomAPI");
        private readonly static HttpClient client = new HttpClient();

        public async Task SendMessage(string conversationId, string message)
        {
            var jsonData = JsonConvert.SerializeObject(
                new
                {
                    ConversationId = conversationId,
                    Message = message
                });
            await client.PostAsync(NexbotUrl, new StringContent(jsonData, Encoding.UTF8, "application/json")).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> GetRegisteredConversation(HangfireCategoryMessageType categoryId)
        {
            return await GetDataAsync<string>(
                new GetRegisteredConversation
                {
                    CategoryId = categoryId
                }).ConfigureAwait(false);
        }
    }
}