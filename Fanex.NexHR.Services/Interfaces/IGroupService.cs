using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.SyncData.SyncNexGoal;
using Fanex.NexHR.Models.SyncData.SyncNexGoalModel;

namespace Fanex.NexHR.Services.Interfaces;

public interface IGroupService
{
    Task<IEnumerable<GroupModel>> GetGroupsByLastModifiedDate(GetGroupsByLastModifiedDateCriteria criteria);
}