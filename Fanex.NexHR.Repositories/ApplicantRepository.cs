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
        public Task<Applicant> GetApplicantById(ICriteria criteria)
        {
            throw new System.NotImplementedException();
        }
    }
}
