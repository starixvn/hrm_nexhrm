namespace Fanex.NexHR.Services
{
    using System.Threading.Tasks;
    using Criteria.Toeic;
    using Interfaces;
    using Models;

    public class ToeicService : IToeicService
    {
        private readonly IBaseService baseService;

        public ToeicService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<Toeic> GetById(GetToeicScoreByEmployeeIdCriteria criteria)
        {
            Toeic toeic = await baseService.GetDataEntityAsync<Toeic>(criteria);
            return toeic;
        }
    }
}