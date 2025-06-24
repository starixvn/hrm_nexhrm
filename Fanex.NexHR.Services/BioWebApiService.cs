namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.XML;
    using Fanex.NexHR.Criteria.TimeSheet;
    using Fanex.NexHR.Models.Bio;
    using Fanex.NexHR.Models.Core;
    using Fanex.NexHR.Models.LogTime;
    using Fanex.NexHR.Services.Interfaces;
    using Newtonsoft.Json;

    public class BioWebApiService : IBioWebApiService
    {
        private readonly HttpClient httpClient;
        private readonly BioWebApiSettings settings;
        private readonly IBaseService baseService;

        public BioWebApiService(
            IHttpClientFactory httpClientFactory,
            BioWebApiSettings settings,
            IBaseService baseService)
        {
            httpClient = httpClientFactory.CreateClient();
            this.settings = settings;
            if (!string.IsNullOrEmpty(settings.ApiKey))
            {
                httpClient.DefaultRequestHeaders.Add("X-API-Key", settings.ApiKey);
            }

            this.baseService = baseService;
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

        public async Task ImportTimeRecordAsync()
        {
            var toDate = DateTimeOffset.Now.DateTime.ToClientTimeZone();
            var fromDate = toDate.AddHours(-6);
            var url = $"{settings.BaseUrl}/TimeRecord?fromDate={fromDate:yyyy-MM-dd HH:mm:ss}&toDate={toDate:yyyy-MM-dd HH:mm:ss}";
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var records = JsonConvert.DeserializeObject<IList<TimeRecordAPI>>(content) ?? new List<TimeRecordAPI>();

                var command = new ImportDataFromBioStarCommand
                {
                    TimeRecordXml = records.ToXML().ToString()
                };

                await baseService.ExecuteNonQueryAsync(command);
            }
        }
    }
}