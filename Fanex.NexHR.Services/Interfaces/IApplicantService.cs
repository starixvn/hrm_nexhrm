namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Applicant;
    using Fanex.NexHR.Criteria.Team;
    using Models.Applicant;

    public interface IApplicantService
    {
        Task<IList<Applicant>> GetAll(GetAllApplicantCriteria criteria);

        Task<Applicant> GetById(GetApplicantByIdCriteria criteria);
    }
}
