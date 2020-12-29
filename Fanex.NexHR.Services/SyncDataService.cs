namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.SyncData;
    using Fanex.NexHR.Models.SyncData;
    using Fanex.NexHR.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class SyncDataService : BaseService, ISyncDataService
    {
        public async Task<IEnumerable<SyncEmployeeModel>> GetEmployeeAsync(bool isNexHrm, DateTime modifiedDate)
        {
            var criteria = new SyncDataGetEmployeeCriteria
            {
                IsNexHRM = isNexHrm,
                LastModifiedDate = modifiedDate
            };

            var employees = await GetDataAsync<SyncEmployeeModel>(criteria);

            return employees;
        }

        public async Task<IEnumerable<SyncDepartmentModel>> GetDepartmentAsync(bool isNexHrm, DateTime modifiedDate)
        {
            var criteria = new SyncDataGetDepartmentCriteria
            {
                IsNexHRM = isNexHrm,
                LastModifiedDate = modifiedDate
            };

            var departments = await GetDataAsync<SyncDepartmentModel>(criteria);

            return departments;
        }

        public async Task<IEnumerable<SyncGroupModel>> GetGroupAsync(bool isNexHrm, DateTime modifiedDate)
        {
            var criteria = new SyncDataGetGroupCriteria
            {
                IsNexHRM = isNexHrm,
                LastModifiedDate = modifiedDate
            };

            var groups = await GetDataAsync<SyncGroupModel>(criteria);

            return groups;
        }

        public async Task<IEnumerable<SyncJobTitleModel>> GetJobTitleAsync(bool isNexHrm)
        {
            var criteria = new SyncDataGetJobTitleCriteria { IsNexHRM = isNexHrm };

            var jobTitles = await GetDataAsync<SyncJobTitleModel>(criteria);

            return jobTitles;
        }
    }
}