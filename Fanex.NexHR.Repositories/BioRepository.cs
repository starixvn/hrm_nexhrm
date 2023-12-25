namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.Bio;

    public class BioRepository : BaseRepository, IBioRepository
    {
        public async Task<IEnumerable<BioTimeChange>> GetAllBioTimeChanges(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<BioTimeChange>(criteria);
            }
        }

        public async Task<IEnumerable<BioTimeChange>> GetAllBioTimeChangesAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<BioTimeChange>(criteria);
            }
        }

        public async Task<IEnumerable<FingerprintAndCardSummary>> GetFingerprintAndCardSummary(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entityList = await db.QueryAsync<FingerprintAndCardSummary, Employee, Team, FingerprintAndCardSummary>(
                        (f, e, g) =>
                        {
                            f.Employee = e;
                            e.Team = g;
                            return f;
                        },
                        splitOn: "EmpId, GroupId",
                        parameterSet: criteria);

                return entityList;
            }
        }

        public async Task<IEnumerable<FingerprintAndCardSummary>> GetFingerprintAndCardSummaryAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var entityList = await db.QueryAsync<FingerprintAndCardSummary, Employee, Team, FingerprintAndCardSummary>(
                        (f, e, g) =>
                        {
                            f.Employee = e;
                            e.Team = g;
                            return f;
                        },
                        splitOn: "EmpId, GroupId",
                        parameterSet: criteria).ConfigureAwait(false);

                return entityList;
            }
        }
    }
}