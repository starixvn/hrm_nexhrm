namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.Postnatal;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.ViewModels;

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
