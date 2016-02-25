namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.LeaveType;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class LeaveTypeService : BaseService, ILeaveTypeService
    {
        public IList<LeaveType> GetAll()
        {
            var criteria = new GetLeaveTypeCriteria();
            IList<LeaveType> leaveTypes = GetData<LeaveType>(criteria);
            return leaveTypes;
        }
    }
}
