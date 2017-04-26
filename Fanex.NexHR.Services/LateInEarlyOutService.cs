namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Criteria.LateInEarlyOut;
    using Interfaces;
    using ViewModels.LateInEarlyOut;

    public class LateInEarlyOutService : BaseService, ILateInEarlyOutService
    {
        public IList<LateInEarlyOutViewModel> GetAllByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria)
        {
            IList<LateInEarlyOutViewModel> lateInEarlyOuts = GetData<LateInEarlyOutViewModel>(criteria);
            return lateInEarlyOuts;
        }

        public LateInEarlyOutViewModel GetById(GetLateInEarlyOutByIdCriteria criteria)
        {
            LateInEarlyOutViewModel lateInEarlyOut = GetDataEntity<LateInEarlyOutViewModel>(criteria);
            return lateInEarlyOut;
        }

        public IList<LateInEarlyOutViewModel> GetAllEmployee(GetAllEmployeeCriteria criteria)
        {
            IList<LateInEarlyOutViewModel> employeeList = GetData<LateInEarlyOutViewModel>(criteria);
            return employeeList;
        }
    }
}
