namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.Applicant;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Models.Applicant;
    using Fanex.NexHR.ViewModels.Applicant;

    public class ApplicantService : IApplicantService
    {
        private readonly IBaseService baseService;

        public ApplicantService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<Source>> GetSourceApplicant(GetSourceApplicantCriteria criteria)
        {
            var sources = await baseService.GetDataAsync<Source>(criteria);
            return sources;
        }

        public async Task<IEnumerable<Source>> GetCVStatusApplicant(GetCVStatusApplicantCriteria criteria)
        {
            var status = await baseService.GetDataAsync<Source>(criteria);
            return status;
        }

         public async Task<IEnumerable<Source>> GetResultApplicant(GetResultApplicantCriteria criteria)
         {
            var result = await baseService.GetDataAsync<Source>(criteria);
            return result;
         }

         public async Task<IEnumerable<Source>> GetProLevelApplicant(GetProLevelApplicantCriteria criteria)
         {
            var proLevel = await baseService.GetDataAsync<Source>(criteria);
            return proLevel;
         }

        public async Task<IEnumerable<Applicant>> GetAll(GetAllApplicantCriteria criteria)
        {
            var applicants = await baseService.GetDataAsync<Applicant>(criteria);
            return applicants;
        }

        public async Task<EditApplicantViewModel> GetById(GetApplicantByIdCriteria criteria)
        {
            var applicant = await baseService.GetDataEntityAsync<EditApplicantViewModel>(criteria);
            return applicant;
        }
    }
}