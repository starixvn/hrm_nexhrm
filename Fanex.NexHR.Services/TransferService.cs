namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.Transfer;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TransferService : ITransferService
    {
        private readonly IBaseService baseService;

        public TransferService(IBaseService baseService)
        {
            this.baseService = baseService;
        }
        public async Task<IEnumerable<Transfer>> GetAll(GetTransferCriteria criteria)
        {
            var transfers = await baseService.GetDataAsync<Transfer>(criteria);
            return transfers.ToList();
        }

        public async Task<Transfer> GetById(GetTransferByIdCriteria criteria)
        {
            var transfer = await baseService.GetDataEntityAsync<Transfer>(criteria);
            return transfer;
        }
    }
}
