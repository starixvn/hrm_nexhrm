namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.TrainingAndDevelopment;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System;
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
        public async Task<IEnumerable<TrainingAndDevelopment>> GetAll(GetTrainingAndDevelopmentCriteria criteria, Employee currentUser)
        {
            var trainingAndDevelopments = await baseService.GetDataAsync<TrainingAndDevelopment>(criteria);
            if (Convert.ToInt32(currentUser.Roles) == Convert.ToInt32(Role.HR)
                && Convert.ToInt32(currentUser.Roles) == Convert.ToInt32(Role.SA))
            {
                trainingAndDevelopments = CheckRuleShowListTrainingAndDevelopment(trainingAndDevelopments, currentUser);
            }
            return trainingAndDevelopments.ToList();
        }

        public async Task<TrainingAndDevelopment> GetById(GetTrainingAndDevelopmentByIdCriteria criteria)
        {
            var trainingAndDevelopments = await baseService.GetDataEntityAsync<TrainingAndDevelopment>(criteria);
            return trainingAndDevelopments;
        }

        public async Task<IList<TrainingAndDevelopment>> GetByEmpId(GetTrainingAndDevelopmentByEmpIdCriteria criteria, Employee currentUser)
        {
            var trainingAndDevelopments = await baseService.GetDataAsync<TrainingAndDevelopment>(criteria);
            if (Convert.ToInt32(currentUser.Roles) == Convert.ToInt32(Role.HR)
                && Convert.ToInt32(currentUser.Roles) == Convert.ToInt32(Role.SA))
            {
                trainingAndDevelopments = CheckRuleShowListTrainingAndDevelopment(trainingAndDevelopments, currentUser);
            }
            return trainingAndDevelopments.ToList();
        }

        public async Task<IList<TrainingAndDevelopmentType>> GetTrainingAndDevelopmentType()
        {
            var trainingAndDevelopments = await baseService.GetDataAsync<TrainingAndDevelopmentType>(new GetTrainingAndDevelopmentTypeCriteria());
            return trainingAndDevelopments.ToList();
        }

        private static List<TrainingAndDevelopment> CheckRuleShowListTrainingAndDevelopment(IEnumerable<TrainingAndDevelopment> trainingAndDevelopments, Employee currentUser)
        {
            var trainingAndDevelopmentFollowRule = new List<TrainingAndDevelopment>();
            foreach (var item in trainingAndDevelopments)
            {
                if (item.Publish.Equals(Constants.Public))
                {
                    trainingAndDevelopmentFollowRule.Add(item);
                }
                else if (item.Publish.Equals(Constants.Protected)
                    && (item.LeaderId.Equals(currentUser.Id) || item.EmpId.Equals(currentUser.Id) || item.GroupId.Equals(currentUser.Team.Id)))
                {
                    trainingAndDevelopmentFollowRule.Add(item);
                }
                else if (item.Publish.Equals(Constants.Private) && item.EmpId.Equals(currentUser.Id))
                {
                    trainingAndDevelopmentFollowRule.Add(item);
                }
            }
            return trainingAndDevelopmentFollowRule;
        }
    }
}
