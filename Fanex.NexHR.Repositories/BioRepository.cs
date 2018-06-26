namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models.Bio;

    public class BioRepository : IBioRepository
    {
        public IEnumerable<BioTimeChange> GetAllBioTimeChanges(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<BioTimeChange>(criteria.GetSpParams());
            }
        }
    }
}
