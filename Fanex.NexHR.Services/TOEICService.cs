namespace Fanex.NexHR.Services
{
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

        public Toeic GetById(GetToeicScoreByEmployeeIdCriteria criteria)
        {
            Toeic toeic = baseService.GetDataEntity<Toeic>(criteria);
            return toeic;
        }
    }
}