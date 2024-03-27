namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Applicant;
    using Fanex.NexHR.ViewModels.Applicant;
    using Models.Applicant;

    public interface IApplicantService
    {
        Task<IEnumerable<Source>> GetSourceApplicant(GetSourceApplicantCriteria criteria);

        Task<IEnumerable<Source>> GetCVStatusApplicant(GetCVStatusApplicantCriteria criteria);

        Task<IEnumerable<Source>> GetGradeApplicant(GetGradeApplicantCriteria criteria);

        Task<IEnumerable<Source>> GetProLevelApplicant(GetProLevelApplicantCriteria criteria);

        Task<IEnumerable<Source>> GetResultApplicant(GetResultApplicantCriteria criteria);

        Task<IEnumerable<Applicant>> GetAll(GetAllApplicantCriteria criteria);

        Task<EditApplicantViewModel> GetById(GetApplicantByIdCriteria criteria);
    }
}
