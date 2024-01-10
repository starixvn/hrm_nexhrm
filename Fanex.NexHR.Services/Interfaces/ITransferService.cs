namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Transfer;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITransferService
    {
        Task<IEnumerable<Transfer>> GetAll(GetTransferCriteria criteria);

        Task<Transfer> GetById(GetTransferByIdCriteria criteria);
    }
}