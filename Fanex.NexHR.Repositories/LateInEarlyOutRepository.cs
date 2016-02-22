namespace Fanex.NexHR.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class LateInEarlyOutRepository : ILateInEarlyOutRepository
    {
        public int Insert(ICriteria criteria)
        {
            string settingKey = criteria.GetSettingKey();
            using (ObjectDb db = new ObjectDb(settingKey))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams());
            }
        }

        public int Update(ICriteria criteria)
        {
            string settingKey = criteria.GetSettingKey();
            using (ObjectDb db = new ObjectDb(settingKey))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams());
            }
        }

        public int Delete(ICriteria criteria)
        {
            string settingKey = criteria.GetSettingKey();
            using (ObjectDb db = new ObjectDb(settingKey))
            {
                return db.ExecuteNonQuery(criteria.GetSpParams());
            }
        }

        public IList<Models.LateInEarlyOut> GetByEmpId(ICriteria criteria)
        {
            string settingKey = criteria.GetSettingKey();
            using (ObjectDb db = new ObjectDb(settingKey))
            {
                return db.Query<LateInEarlyOut>(criteria.GetSpParams()).ToList();
            }
        }
    }
}
