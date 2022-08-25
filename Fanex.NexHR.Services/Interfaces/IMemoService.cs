namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Memo;
    using Models.Memo;

    public interface IMemoService
    {
        Task<IEnumerable<Memo>> GetAll(GetAllMemoCriteria criteria);

        Task<Memo> GetById(GetMemoByIdCriteria criteria);

        void SendMail(string subject, string memoId);
    }
}
