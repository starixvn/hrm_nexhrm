namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Postnatal;
    using ViewModels.Postnatal;

    public interface IPostnatalService
    {
        Task<IEnumerable<PostnatalViewModel>> GetAll(GetAllPostnatalCriteria criteria);

        Task<PostnatalViewModel> GetById(GetByIdPostnatalCriteria criteria);
    }
}
