namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Models.SyncData;
    using Fanex.NexHR.Models.SyncData.Flow;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISyncDataService
    {
        Task<IEnumerable<SyncEmployeeModel>> GetEmployeeAsync(bool isNexcel, DateTime modifiedDate);

        Task<IEnumerable<SyncDepartmentModel>> GetDepartmentAsync(bool isNexcel, DateTime modifiedDate);

        Task<IEnumerable<SyncGroupModel>> GetGroupAsync(bool isNexcel, DateTime modifiedDate);

        Task<IEnumerable<SyncJobTitleModel>> GetJobTitleAsync(bool isNexcel);

        Task<EpaFlow> GetFlowAsync(bool isNexcel);
    }
}