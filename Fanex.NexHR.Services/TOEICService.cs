namespace Fanex.NexHR.Services
{
    using Criteria.Toeic;
    using Interfaces;
    using Models;

    public class ToeicService : BaseService, IToeicService
    {
        public Toeic GetById(GetToeicScoreByEmployeeIdCriteria criteria)
        {            
            Toeic toeic = GetDataEntity<Toeic>(criteria);
            return toeic;
        }
    }
}
