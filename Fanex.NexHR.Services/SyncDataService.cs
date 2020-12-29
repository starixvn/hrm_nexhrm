namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.SyncData;
    using Fanex.NexHR.Models.SyncData;
    using Fanex.NexHR.Models.SyncData.Flow;
    using Fanex.NexHR.Services.Interfaces;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using System.Threading.Tasks;

    public class SyncDataService : BaseService, ISyncDataService
    {
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
            var restClient = new RestClient { Encoding = Encoding.UTF8 };

            restClient.BaseUrl = isNexcel
                ? new Uri(ConfigurationManager.AppSettings["NexGoalBaseUrl"])
                : new Uri(ConfigurationManager.AppSettings["LumiGoalUrl"]);

            var request = new RestRequest(ConfigurationManager.AppSettings["FlowUrl"], Method.GET);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            var response = await restClient.ExecuteGetTaskAsync<EpaFlow>(request);

            return response.Data;
        }
    }
}