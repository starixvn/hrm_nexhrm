using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.Applicant;
using Fanex.NexHR.Services.Interfaces;
using Fanex.NexHR.Repositories;
using Fanex.NexHR.Repositories.Interfaces;
using Fanex.NexHR.Models.Applicant;
using Fanex.NexHR.Criteria.Team;


namespace Fanex.NexHR.Services
{
    public class ApplicantService : IApplicantService
    {
        private readonly IApplicantRepository applicantRepository;
        private readonly IBaseService baseService;

        public ApplicantService(
            IApplicantRepository applicantRepository,
            IBaseService baseService
        )
        {
            this.applicantRepository = applicantRepository ?? new ApplicantRepository();
            this.baseService = baseService;
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

        public async Task<Applicant> GetById(GetApplicantByIdCriteria criteria)
        {
             Applicant applicant = null;
            if (criteria != null && criteria.IsValid())
            {
                applicant = await baseService.GetDataEntityAsync<Applicant>(criteria);
            }

            return applicant;
        }
    }
}