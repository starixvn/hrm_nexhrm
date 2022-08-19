namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
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

        public IList<PostnatalViewModel> GetAll(GetAllPostnatalCriteria criteria)
        {
            List<PostnatalViewModel> listPostanal = baseService.GetData<PostnatalViewModel>(criteria).ToList();
            return listPostanal;
        }

        public PostnatalViewModel GetById(GetByIdPostnatalCriteria criteria)
        {
            PostnatalViewModel detailPostanal = baseService.GetDataEntity<PostnatalViewModel>(criteria);
            return detailPostanal;
        }
    }
}