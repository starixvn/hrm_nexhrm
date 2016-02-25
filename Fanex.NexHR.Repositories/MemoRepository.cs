namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    public class MemoRepository : IMemoRepository
    {
        /// <summary>
        /// Get all Memo.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Memo Entity.</returns>
        public IList<Memo> GetAll(ICriteria criteria)
        {
            IList<Memo> lstMemo = new List<Memo>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                lstMemo = db.Query<Memo>(criteria.GetSpParams()).ToList();
            }

            return lstMemo;
        }

        /// <summary>
        /// Get memo by ID.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The memo object.</returns>
        public Memo GetById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<Memo>(criteria.GetSpParams());
            }
        }
    }
}
