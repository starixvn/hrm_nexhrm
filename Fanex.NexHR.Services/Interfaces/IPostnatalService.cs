namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Postnatal;
    using Fanex.NexHR.ViewModels.Postnatal;
    using System.Collections.Generic;

    public interface IPostnatalService
    {
        IList<PostnatalViewModel> GetAll(GetAllPostnatalCriteria criteria);

        PostnatalViewModel GetById(GetByIdPostnatalCriteria criteria);
    }
}
