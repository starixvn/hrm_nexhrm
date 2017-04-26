namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.Postnatal;
    using ViewModels.Postnatal;

    public interface IPostnatalService
    {
        IList<PostnatalViewModel> GetAll(GetAllPostnatalCriteria criteria);

        PostnatalViewModel GetById(GetByIdPostnatalCriteria criteria);
    }
}
