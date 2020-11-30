namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Models.SyncData;

    public interface ISyncDataService
    {
        Task<IEnumerable<SyncEmployeeModel>> GetEmployeeAsync(DateTime modifiedDate);

        Task<IEnumerable<SyncDepartmentModel>> GetDepartmentAsync(DateTime modifiedDate);

        Task<IEnumerable<SyncGroupModel>> GetGroupAsync(DateTime modifiedDate);

        Task<IEnumerable<SyncJobTitleModel>> GetJobTitleAsync();
    }
}