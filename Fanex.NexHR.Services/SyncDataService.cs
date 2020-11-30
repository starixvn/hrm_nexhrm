namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.SyncData;
    using Fanex.NexHR.Models.SyncData;
    using Fanex.NexHR.Services.Interfaces;

    public class SyncDataService : BaseService, ISyncDataService
    {
        public async Task<IEnumerable<SyncEmployeeModel>> GetEmployeeAsync(DateTime modifiedDate)
        {
            var criteria = new SyncDataGetEmployeeCriteria
            {
                LastModifiedDate = modifiedDate
            };

            var employees = await GetDataAsync<SyncEmployeeModel>(criteria);

            return employees;
        }

        public async Task<IEnumerable<SyncDepartmentModel>> GetDepartmentAsync(DateTime modifiedDate)
        {
            var criteria = new SyncDataGetDepartmentCriteria
            {
                LastModifiedDate = modifiedDate
            };

            var departments = await GetDataAsync<SyncDepartmentModel>(criteria);

            return departments;
        }

        public async Task<IEnumerable<SyncGroupModel>> GetGroupAsync(DateTime modifiedDate)
        {
            var criteria = new SyncDataGetGroupCriteria
            {
                LastModifiedDate = modifiedDate
            };

            var groups = await GetDataAsync<SyncGroupModel>(criteria);

            return groups;
        }

        public async Task<IEnumerable<SyncJobTitleModel>> GetJobTitleAsync()
        {
            var criteria = new SyncDataGetJobTitleCriteria();

            var jobTitles = await GetDataAsync<SyncJobTitleModel>(criteria);

            return jobTitles;
        }
    }
}