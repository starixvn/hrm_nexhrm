namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.TrainingAndDevelopment;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITrainingAndDevelopmentService
    {
        Task<IEnumerable<TrainingAndDevelopment>> GetAll(GetTrainingAndDevelopmentCriteria criteria, Employee currentUser);
        Task<TrainingAndDevelopment> GetById(GetTrainingAndDevelopmentByIdCriteria criteria);
        Task<IList<TrainingAndDevelopment>> GetByEmpId(GetTrainingAndDevelopmentByEmpIdCriteria criteria, Employee currentUser);
        Task<IList<TrainingAndDevelopmentType>> GetTrainingAndDevelopmentType();
    }
}