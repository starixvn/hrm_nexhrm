namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
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
   	
        public IList<FingerprintAndCardSummary> GetFingerprintAndCardSummary(ICriteria criteria)
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

        public IList<Fingerprint> GetFingerprintDetailByEmpId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<Fingerprint, Employee, Team, Fingerprint>(
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

        public IList<Card> GetCardDetailByEmpId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<Card, Employee, Team, Card>(
                        (c, e, g) =>
                        {
                            c.Employee = e;
                            e.Team = g;
                            return c;
                        },
                        splitOn: "EmpId, GroupId",
                        parameterSet: criteria.GetSpParams())
                    .ToList();

                return entityList;
            }
        }
    }
}