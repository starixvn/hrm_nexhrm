// --------------------------------------------------------------------------
// <copyright file="MemoRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Memo repository.
    /// </summary>
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

        /// <summary>
        /// Delete memo.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The result.</returns>
        public bool Delete(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }
        }

        /// <summary>
        /// Insert memo.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The result.</returns>
        public int Insert(InsertMemoCriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return (int)db.ExecuteScalar(criteria.GetSpParams());
            }
        }

        /// <summary>
        /// Update memo.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The result.</returns>
        public bool Update(UpdateMemoCriteria criteria)
        {
            bool updated = false;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                updated = db.ExecuteNonQuery(criteria.GetSpParams()) != 0;
            }

            return updated;
        }
    }
}
