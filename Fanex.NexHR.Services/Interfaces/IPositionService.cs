namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.Position;
    using Fanex.NexHR.ViewModels;
    using Models;

    public interface IPositionService
    {
        Task<IEnumerable<Position>> GetAll(GetAllPositionCriteria criteria);
        Task<EditPositionViewModel> GetById(GetPositionByIdCriteria criteria);
    }
}
