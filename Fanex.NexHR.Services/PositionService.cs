using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.Position;
using Fanex.NexHR.Services.Interfaces;
using Fanex.NexHR.Models;

namespace Fanex.NexHR.Services
{
    public class PositionService : IPositionService
    {
        private readonly IBaseService baseService;


        public PositionService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<Position>> GetAll()
        {
            var criteria = new GetAllPositionCriteria();
            IEnumerable<Position> positions = await baseService.GetDataAsync<Position>(criteria);
            return positions;
        }
    }
}