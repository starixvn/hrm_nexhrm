namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Fanex.NexHR.Models.Core;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Models.LogTime;
    using Newtonsoft.Json;

    public class BioWebApiService : IBioWebApiService
    {
        private readonly HttpClient httpClient;
        private readonly BioWebApiSettings settings;

        public BioWebApiService(IHttpClientFactory httpClientFactory, BioWebApiSettings settings)
        {
            httpClient = httpClientFactory.CreateClient();
            this.settings = settings;
            if (!string.IsNullOrEmpty(settings.ApiKey))
            {
                httpClient.DefaultRequestHeaders.Add("X-API-Key", settings.ApiKey);
            }
        }

        public async Task<List<TimeEntryModel>> GetTimeEntriesAsync(DateTime date)
        {
            var url = $"{settings.BaseUrl}/redmine/time-entries?date={date:yyyy-MM-dd}";
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TimeEntryModel>>(content) ?? new List<TimeEntryModel>();
            }
            return new List<TimeEntryModel>();
        }
    }
}