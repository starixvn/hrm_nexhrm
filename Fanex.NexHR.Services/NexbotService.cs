namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Enums;
    using Criteria.Hangfire;
    using Criteria.SkypeAccount;
    using Interfaces;
    using Models.NexBot;
    using Models.SkypeAccount;
    using Repositories;
    using Repositories.Interfaces;
    using RestSharp;

    public class NexbotService : BaseService, INexbotService
    {
        private readonly IRestClient webClient;
        private readonly INexbotRepository nexbotRepository;

        public NexbotService()
        {
            webClient = new RestClient { Encoding = Encoding.UTF8 };
            nexbotRepository = new NexbotRepository();
        }

        public async Task SendMessage(string conversationId, string message)
        {
            var token = await GetToken().ConfigureAwait(false);
            await ForwardToBot(message, conversationId, token).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> GetRegisteredConversation(HangfireCategoryMessageType categoryId)
        {
            return await GetDataAsync<string>(
                new GetRegisteredConversation
                {
                    CategoryId = categoryId
                }).ConfigureAwait(false);
        }

        private async Task ForwardToBot(string message, string conversationId, Token token)
        {
            webClient.BaseUrl = NexBotConfiguration.BotServiceUrl;

            var request = new RestRequest("/messages/forward", Method.POST);
            request.AddHeader("Authorization", $"Bearer {token.AccessToken}");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("message", message);
            request.AddParameter("conversationId", conversationId);

            await webClient.ExecutePostTaskAsync(request).ConfigureAwait(false);
        }

        private async Task<Token> GetToken()
        {
            webClient.BaseUrl = NexBotConfiguration.TokenUrl;

            var request = new RestRequest(Method.POST);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", NexBotConfiguration.ClientId);
            request.AddParameter("client_secret", NexBotConfiguration.ClientPassword);
            request.AddParameter("scope", $"{NexBotConfiguration.ClientId}/.default");

            request.AddHeader("Content-type", "application/x-www-form-urlencoded");

            var response = await webClient.ExecutePostTaskAsync<Token>(request).ConfigureAwait(false);

            return response.Data;
        }

        public IEnumerable<EmployeeSkypeAccount> GetEmployeeSkypeAccount(GetEmployeeSkypeAccountCriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return nexbotRepository.GetEmployeeSkypeAccount(criteria);
            }

            return new List<EmployeeSkypeAccount>();
        }
    }
}