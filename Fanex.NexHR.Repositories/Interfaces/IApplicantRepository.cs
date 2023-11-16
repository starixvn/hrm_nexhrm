namespace Fanex.NexHR.Repositories.Interfaces
{
    using Data.Repository;
    using Fanex.NexHR.Models.Applicant;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IApplicantRepository
    {
        Task<IList<Applicant>> GetAll(ICriteria criteria);

        Task<Applicant> GetApplicantById(ICriteria criteria);
    }
}
