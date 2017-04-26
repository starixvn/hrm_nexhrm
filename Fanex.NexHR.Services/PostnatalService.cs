namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria.Postnatal;
    using Interfaces;
    using ViewModels.Postnatal;

    public class PostnatalService : BaseService, IPostnatalService
    {
        public IList<PostnatalViewModel> GetAll(GetAllPostnatalCriteria criteria)
        {
            List<PostnatalViewModel> listPostanal = GetData<PostnatalViewModel>(criteria).ToList();
            return listPostanal;
        }

        public PostnatalViewModel GetById(GetByIdPostnatalCriteria criteria)
        {
            PostnatalViewModel detailPostanal = GetDataEntity<PostnatalViewModel>(criteria);
            return detailPostanal;
        }
    }
}
