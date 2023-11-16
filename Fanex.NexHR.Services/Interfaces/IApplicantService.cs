namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Applicant;
    using Models.Applicant;

    public interface IApplicantService
    {
        Task<IList<Applicant>> GetAll(GetAllApplicantCriteria criteria);

    }
}
