namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Criteria.LeaveType;
    using Interfaces;
    using Models;

    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly IBaseService baseService;

        public LeaveTypeService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<LeaveType> GetAll()
        {
            var criteria = new GetLeaveTypeCriteria();
            IList<LeaveType> leaveTypes = baseService.GetData<LeaveType>(criteria);
            return leaveTypes;
        }
    }
}