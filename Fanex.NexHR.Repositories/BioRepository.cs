namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.Bio;

    public class BioRepository : BaseRepository, IBioRepository
    {
        public IEnumerable<BioTimeChange> GetAllBioTimeChanges(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<BioTimeChange>(criteria.GetSpParams());
            }
        }

        public async Task<IEnumerable<BioTimeChange>> GetAllBioTimeChangesAsync(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return await db.QueryAsync<BioTimeChange>(criteria.GetSpParams()).ConfigureAwait(false);
            }
        }

        public IEnumerable<FingerprintAndCardSummary> GetFingerprintAndCardSummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<FingerprintAndCardSummary, Employee, Team, FingerprintAndCardSummary>(
                        (f, e, g) =>
                        {
                            f.Employee = e;
                            e.Team = g;
                            return f;
                        },
                        splitOn: "EmpId, GroupId",
                        parameterSet: criteria.GetSpParams())
                    .ToList();

                return entityList;
            }
        }

        public async Task<IEnumerable<FingerprintAndCardSummary>> GetFingerprintAndCardSummaryAsync(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = await db.QueryAsync<FingerprintAndCardSummary, Employee, Team, FingerprintAndCardSummary>(
                        (f, e, g) =>
                        {
                            f.Employee = e;
                            e.Team = g;
                            return f;
                        },
                        splitOn: "EmpId, GroupId",
                        parameterSet: criteria.GetSpParams()).ConfigureAwait(false);

                return entityList;
            }
        }
    }
}