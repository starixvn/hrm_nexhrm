namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models.Memo;
    using System.Collections.Generic;

    public interface IMemoService
    {
        IList<Memo> GetAll(GetAllMemoCriteria criteria);

        Memo GetById(GetMemoByIdCriteria criteria);

        void SendMail(string subject, string memoId);
    }
}
