namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.TrainingAndDevelopment;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TrainingAndDevelopmentService : ITrainingAndDevelopmentService
    {
        private readonly IBaseService baseService;

        public TrainingAndDevelopmentService(IBaseService baseService)
        {
            this.baseService = baseService;
        }
        public async Task<IEnumerable<TrainingAndDevelopment>> GetAll(GetTrainingAndDevelopmentCriteria criteria, UserSession currentUser)
        {
            var trainingAndDevelopments = await baseService.GetDataAsync<TrainingAndDevelopment>(criteria);
            return trainingAndDevelopments.ToList();
        }

        public async Task<TrainingAndDevelopment> GetById(GetTrainingAndDevelopmentByIdCriteria criteria)
        {
            var trainingAndDevelopments = await baseService.GetDataEntityAsync<TrainingAndDevelopment>(criteria);
            return trainingAndDevelopments;
        }

        public async Task<IList<TrainingAndDevelopment>> GetByEmpId(GetTrainingAndDevelopmentByEmpIdCriteria criteria, UserSession currentUser)
        {
            var trainingAndDevelopments = await baseService.GetDataAsync<TrainingAndDevelopment>(criteria);
            return trainingAndDevelopments.ToList();
        }

        public async Task<IList<TrainingAndDevelopmentType>> GetTrainingAndDevelopmentType()
        {
            var trainingAndDevelopments = await baseService.GetDataAsync<TrainingAndDevelopmentType>(new GetTrainingAndDevelopmentTypeCriteria());
            return trainingAndDevelopments.ToList();
        }
    }
}
