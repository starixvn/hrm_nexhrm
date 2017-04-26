namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Data.Repository;
    using Models.Request;

    public interface IRequestRepository
    {
        IList<RequestEntity> GetRequest(ICriteria criteria);

        RequestEntity GetRequestEntity(ICriteria criteria);
    }
}
