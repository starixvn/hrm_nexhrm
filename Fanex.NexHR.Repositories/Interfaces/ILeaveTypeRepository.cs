namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public interface ILeaveTypeRepository
    {
        IList<LeaveType> GetAll(ICriteria criteria);
    }
}
