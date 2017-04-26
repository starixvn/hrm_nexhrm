namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.Memo;
    using Models.Memo;

    public interface IMemoService
    {
        IList<Memo> GetAll(GetAllMemoCriteria criteria);

        Memo GetById(GetMemoByIdCriteria criteria);

        void SendMail(string subject, string memoId);
    }
}
