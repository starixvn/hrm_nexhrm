namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.WorkFromHomeRequest;
    using Fanex.NexHR.Models;

    public interface IWorkFromHomeRequest
    {
        IList<WorkFromHomeRequest> GetAll(GetWorkFromHomeRequestCriteria criteria);
        WorkFromHomeRequest GetById(int id);
        CommandResult Insert(InsertWorkFromHomeRequestCriteria criteria);

        //CommandResult Update(UpdateEmployeeWorkFromHomeCriteria criteria);

        //CommandResult Delete(int id);
    }
}
