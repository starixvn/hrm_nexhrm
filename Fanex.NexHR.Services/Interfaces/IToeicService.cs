namespace Fanex.NexHR.Services.Interfaces
{
    using System.Threading.Tasks;
    using Criteria.Toeic;
    using Models;

    public interface IToeicService
    {
        Task<Toeic> GetById(GetToeicScoreByEmployeeIdCriteria criteria);
    }
}
