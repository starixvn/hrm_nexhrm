using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.Applicant;
using Fanex.NexHR.Services.Interfaces;
using Fanex.NexHR.Models.Applicant;
using Fanex.NexHR.ViewModels.Applicant;

namespace Fanex.NexHR.Services
{
    public class ApplicantService : IApplicantService
    {
        private readonly IBaseService baseService;

        public ApplicantService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<Applicant>> GetAll(GetAllApplicantCriteria criteria)
        {
            IEnumerable<Applicant> applicants = null;

            if (criteria != null && criteria.IsValid())
            {
                applicants = await baseService.GetDataAsync<Applicant>(criteria);
            }

            return applicants ?? new List<Applicant>();
        }

        public async Task<EditApplicantViewModel> GetById(GetApplicantByIdCriteria criteria)
        {
             EditApplicantViewModel applicant = null;
            if (criteria != null && criteria.IsValid())
            {
                applicant = await baseService.GetDataEntityAsync<EditApplicantViewModel>(criteria);
            }

            return applicant;
        }
    }
}