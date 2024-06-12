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

        public async Task<IEnumerable<Source>> GetSourceApplicant(GetSourceApplicantCriteria criteria)
        {
            IEnumerable<Source> sources = null;

            if (criteria != null && criteria.IsValid())
            {
                sources = await baseService.GetDataAsync<Source>(criteria);
            }

            return sources ?? new List<Source>();
        }

        public async Task<IEnumerable<Source>> GetCVStatusApplicant(GetCVStatusApplicantCriteria criteria)
        {
            IEnumerable<Source> status = null;

            if (criteria != null && criteria.IsValid())
            {
                status = await baseService.GetDataAsync<Source>(criteria);
            }

            return status ?? new List<Source>();
        }

         public async Task<IEnumerable<Source>> GetResultApplicant(GetResultApplicantCriteria criteria)
        {
            IEnumerable<Source> result = null;

            if (criteria != null && criteria.IsValid())
            {
                result = await baseService.GetDataAsync<Source>(criteria);
            }

            return result ?? new List<Source>();
        }

         public async Task<IEnumerable<Source>> GetProLevelApplicant(GetProLevelApplicantCriteria criteria)
        {
            IEnumerable<Source> proLevel = null;

            if (criteria != null && criteria.IsValid())
            {
                proLevel = await baseService.GetDataAsync<Source>(criteria);
            }

            return proLevel ?? new List<Source>();
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