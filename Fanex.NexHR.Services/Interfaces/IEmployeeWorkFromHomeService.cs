namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;

    public interface IEmployeeWorkFromHomeService
    {
        IList<EmployeeWorkFromHome> GetAll(GetEmployeeWorkFromHomeCriteria criteria);
        EmployeeWorkFromHome GetById(int id);
        CommandResult Insert(InsertEmployeeWorkFromHomeCriteria criteria);

        CommandResult Update(UpdateEmployeeWorkFromHomeCriteria criteria);

        CommandResult Delete(DeleteEmployeeWorkFromHomeCriteria criteria);
        CommandResult ProcessApproveAndReject(EmployeeWorkFromHomeApproveAndRejectRequestCriteria criteria);
        CommandResult ProcessDisapprove(EmployeeWorkFromHomeDisApproveRequestCriteria criteria);
    }
}
