namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.Criteria.LateInEarlyOut;
    using Fanex.NexHR.Models;

    public interface ILateInEarlyOutService
    {
        int Insert(InsertLateInEarlyOutCriteria criteria);

        int Update(UpdateLateInEarlyOutCriteria criteria);
        
        int Delete(DeleteLateInEarlyOutCriteria criteria);

        IList<LateInEarlyOut> GetByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria);
    }
}