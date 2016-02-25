namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Toeic;
    using Fanex.NexHR.Models;

    public interface IToeicService
    {
        Toeic GetById(GetToeicScoreByEmployeeIdCriteria criteria);
    }
}
