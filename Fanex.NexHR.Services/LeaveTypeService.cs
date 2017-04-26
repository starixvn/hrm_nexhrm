namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Criteria.LeaveType;
    using Interfaces;
    using Models;

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
