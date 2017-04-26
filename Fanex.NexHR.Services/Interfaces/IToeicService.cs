namespace Fanex.NexHR.Services.Interfaces
{
    using Criteria.Toeic;
    using Models;

    public interface IToeicService
    {
        Toeic GetById(GetToeicScoreByEmployeeIdCriteria criteria);
    }
}
