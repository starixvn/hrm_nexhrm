namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria.Postnatal;
    using Interfaces;
    using ViewModels.Postnatal;

    public class PostnatalService : IPostnatalService
    {
        private readonly IBaseService baseService;

        public PostnatalService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<PostnatalViewModel>> GetAll(GetAllPostnatalCriteria criteria)
        {
            IEnumerable<PostnatalViewModel> listPostanal = await baseService.GetDataAsync<PostnatalViewModel>(criteria);
            return listPostanal.ToList();
        }

        public async Task<PostnatalViewModel> GetById(GetByIdPostnatalCriteria criteria)
        {
            PostnatalViewModel detailPostanal = await baseService.GetDataEntityAsync<PostnatalViewModel>(criteria);
            return detailPostanal;
        }
    }
}