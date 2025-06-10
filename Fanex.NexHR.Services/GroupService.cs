using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.SyncData.SyncNexGoal;
using Fanex.NexHR.Models.SyncData.SyncNexGoalModel;
using Fanex.NexHR.Services.Interfaces;

namespace Fanex.NexHR.Services;

public class GroupService(IBaseService baseService) : IGroupService
{
    public async Task<IEnumerable<GroupModel>> GetGroupsByLastModifiedDate(GetGroupsByLastModifiedDateCriteria criteria)
    {
        return await baseService.GetDataAsync<GroupModel>(criteria);
    }
}