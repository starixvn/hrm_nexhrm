namespace Fanex.NexHR.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels.LateInEarlyOut;
    using System.Collections.Generic;
    using System.Linq;

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

        public IList<LateInEarlyOutViewModel> GetByEmpId(ICriteria criteria)
        {
            string settingKey = criteria.GetSettingKey();
            using (ObjectDb db = new ObjectDb(settingKey))
            {
                return db.Query<LateInEarlyOutViewModel>(criteria.GetSpParams()).ToList();
            }
        }
    }
}
