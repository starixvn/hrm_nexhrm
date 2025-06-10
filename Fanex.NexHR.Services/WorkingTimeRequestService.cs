using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.SyncData.SyncNexGoal;
using Fanex.NexHR.Models.SyncData.SyncNexGoalModel;
using Fanex.NexHR.Services.Interfaces;

namespace Fanex.NexHR.Services;

public class WorkingTimeRequestService(IBaseService baseService) : IWorkingTimeRequestService
{
    public async Task<IEnumerable<WorkingTimeRequestModel>> GetWorkingTimeRequestsByLastModifiedDate(GetWorkingTimeRequestsByLastModifiedDateCriteria criteria)
    {
        return await baseService.GetDataAsync<WorkingTimeRequestModel>(criteria);
    }
}