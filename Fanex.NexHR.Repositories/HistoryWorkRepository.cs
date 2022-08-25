namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.HistoryWork;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    public class HistoryWorkRepository : IHistoryWorkRepository
    {
        public HistoryWork EditHistoryWork(EditHistoryWorkCriteria criteria)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IList<HistoryWork>> GetFilterHistoryWork(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var historyWork = await db.QueryAsync<HistoryWork>(criteria);

                return historyWork.ToList();
            }
        }

        public async Task<IList<HistoryWork>> GetHistoryWorkByEmpId(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var historyWork = await db.QueryAsync<HistoryWork>(criteria);

                return historyWork.ToList();
            }
        }
    }   
}