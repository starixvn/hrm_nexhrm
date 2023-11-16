namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Fanex.NexHR.Models.Applicant;
    using Interfaces;

    public class ApplicantRepository : IApplicantRepository
    {
        public async Task<IList<Applicant>> GetAll(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                 var applicants = await db.QueryAsync<Applicant>(criteria);

                return applicants.ToList();
            }
        }

        public Task<Applicant> GetApplicantById(ICriteria criteria)
        {
            throw new System.NotImplementedException();
        }
    }
}
