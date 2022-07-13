namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.SyncData;
    using Fanex.NexHR.Models.SyncData;
    using Fanex.NexHR.Models.SyncData.Flow;
    using Fanex.NexHR.Services.Interfaces;
    using Newtonsoft.Json;

    public class SyncDataService : BaseService, ISyncDataService
    {
        private readonly HttpClient httpClient;
        private readonly SyncDataSettings settings;

        public SyncDataService(IHttpClientFactory httpClientFactory, SyncDataSettings settings)
        {
            httpClient = httpClientFactory.CreateClient();
            this.settings = settings;
        }

        public async Task<IEnumerable<SyncEmployeeModel>> GetEmployeeAsync(bool isNexcel, DateTime modifiedDate)
        {
            var criteria = new SyncDataGetEmployeeCriteria
            {
                IsNexHRM = isNexcel,
                LastModifiedDate = modifiedDate
            };

            var employees = await GetDataAsync<SyncEmployeeModel>(criteria);

            return employees;
        }

        public async Task<IEnumerable<SyncDepartmentModel>> GetDepartmentAsync(bool isNexcel, DateTime modifiedDate)
        {
            var criteria = new SyncDataGetDepartmentCriteria
            {
                IsNexHRM = isNexcel,
                LastModifiedDate = modifiedDate
            };

            var departments = await GetDataAsync<SyncDepartmentModel>(criteria);

            return departments;
        }

        public async Task<IEnumerable<SyncGroupModel>> GetGroupAsync(bool isNexcel, DateTime modifiedDate)
        {
            var criteria = new SyncDataGetGroupCriteria
            {
                IsNexHRM = isNexcel,
                LastModifiedDate = modifiedDate
            };

            var groups = await GetDataAsync<SyncGroupModel>(criteria);

            return groups;
        }

        public async Task<IEnumerable<SyncJobTitleModel>> GetJobTitleAsync(bool isNexcel)
        {
            var criteria = new SyncDataGetJobTitleCriteria { IsNexHRM = isNexcel };

            var jobTitles = await GetDataAsync<SyncJobTitleModel>(criteria);

            return jobTitles;
        }

        public async Task<EpaFlow> GetFlowAsync(bool isNexcel)
        {
            string baseUrl = isNexcel ? settings.NexGoalBaseUrl : settings.LumiGoalBaseUrl;
            Uri url = new($"{baseUrl}{settings.FlowUrl}");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("content-type", "application/x-www-form-urlencoded");

            using var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();
                var epaFlow = JsonConvert.DeserializeObject<EpaFlow>(jsonResult);
                return epaFlow;
            }

            return default;
        }
    }
}