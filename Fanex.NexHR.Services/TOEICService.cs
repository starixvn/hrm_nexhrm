namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.Toeic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class ToeicService : BaseService, IToeicService
    {
        public Toeic GetById(GetToeicScoreByEmployeeIdCriteria criteria)
        {
            Toeic toeic = GetDataEntity<Toeic>(criteria);
            return toeic;
        }
    }
}
