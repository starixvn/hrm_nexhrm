namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Models.SyncData;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISyncDataService
    {
        Task<IEnumerable<SyncEmployeeModel>> GetEmployeeAsync(bool isNexHrm, DateTime modifiedDate);

        Task<IEnumerable<SyncDepartmentModel>> GetDepartmentAsync(bool isNexHrm, DateTime modifiedDate);

        Task<IEnumerable<SyncGroupModel>> GetGroupAsync(bool isNexHrm, DateTime modifiedDate);

        Task<IEnumerable<SyncJobTitleModel>> GetJobTitleAsync(bool isNexHrm);
    }
}