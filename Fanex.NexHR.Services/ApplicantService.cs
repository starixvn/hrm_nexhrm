using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.Applicant;
using Fanex.NexHR.Services.Interfaces;
using Fanex.NexHR.Repositories;
using Fanex.NexHR.Repositories.Interfaces;
using Fanex.NexHR.Models.Applicant;


namespace Fanex.NexHR.Services
{
    public class ApplicantService : IApplicantService
    {
        private readonly IApplicantRepository applicantRepository;

        public ApplicantService(IApplicantRepository applicantRepository)
        {
            this.applicantRepository = applicantRepository ?? new ApplicantRepository();
        }

        public async Task<IList<Applicant>> GetAll(GetAllApplicantCriteria criteria)
        {
            IList<Applicant> applicants = null;

            if (criteria != null && criteria.IsValid())
            {
                applicants = await applicantRepository.GetAll(criteria);
            }

            return applicants ?? new List<Applicant>();
        }
    }
}