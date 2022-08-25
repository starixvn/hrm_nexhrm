namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
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

        public async Task<IEnumerable<LeaveType>> GetAll()
        {
            var criteria = new GetLeaveTypeCriteria();
            IEnumerable<LeaveType> leaveTypes = await baseService.GetDataAsync<LeaveType>(criteria);
            return leaveTypes;
        }
    }
}