namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    public interface IRequestRepository
    {
        IList<RequestEntity> GetRequest(ICriteria criteria);

        RequestEntity GetRequestEntity(ICriteria criteria);

        decimal GetNumberRequestInMonth(ICriteria criteria);
    }
}
