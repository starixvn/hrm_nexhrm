using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.Position;
using Fanex.NexHR.Services.Interfaces;
using Fanex.NexHR.Models;
using Fanex.NexHR.ViewModels;
using System.Linq;

namespace Fanex.NexHR.Services
{
    public class PositionService : IPositionService
    {
        private readonly IBaseService baseService;


        public PositionService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<Position>> GetAll(GetAllPositionCriteria criteria)
        {
            IEnumerable<Position> positions = await baseService.GetDataAsync<Position>(criteria);
            return positions.OrderBy(x => x.DisplayOrder).ToList();
        }

        public async Task<EditPositionViewModel> GetById(GetPositionByIdCriteria criteria)
        {
            EditPositionViewModel position = null;
            if (criteria != null && criteria.IsValid())
            {
                position = await baseService.GetDataEntityAsync<EditPositionViewModel>(criteria);
            }
            return position;
        }
    }
}