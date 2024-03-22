namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Applicant;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels.Applicant;
    using Models.Applicant;

    public interface IApplicantService
    {
        Task<IEnumerable<EmployeeApplicant>> GetSourceApplicant(GetSourceApplicantCriteria criteria);

        Task<IEnumerable<Applicant>> GetAll(GetAllApplicantCriteria criteria);

        Task<EditApplicantViewModel> GetById(GetApplicantByIdCriteria criteria);
    }
}
