namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Repository;
    using Models.Request;

    public interface IRequestRepository
    {
        Task<IList<RequestEntity>> GetRequest(ICriteria criteria);

        Task<RequestEntity> GetRequestEntity(ICriteria criteria);
    }
}
