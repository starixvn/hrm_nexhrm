namespace Fanex.NexHR.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.HistoryWork;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class HistoryWorkRepository : IHistoryWorkRepository
    {
        public HistoryWork EditHistoryWork(EditHistoryWorkCriteria criteria)
        {
            throw new System.NotImplementedException();
        }

        public IList<HistoryWork> GetFilterHistoryWork(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var historyWork = db.Query<HistoryWork>(criteria);

                return historyWork.ToList();
            }
        }

        public IList<HistoryWork> GetHistoryWorkByEmpId(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var historyWork = db.Query<HistoryWork>(criteria);

                return historyWork.ToList();
            }
        }
    }   
}