namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.LateInEarlyOut;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Fanex.NexHR.Repositories.Interfaces;
    using Interfaces;
    using ViewModels.LateInEarlyOut;

    public class LateInEarlyOutService : ILateInEarlyOutService
    {
        private readonly ILateInEarlyOutRepository lateInEarlyOutrepository;
        private readonly IBaseService baseService;

        public LateInEarlyOutService(ILateInEarlyOutRepository lateInEarlyOutrepository, IBaseService baseService)
        {
            this.lateInEarlyOutrepository = lateInEarlyOutrepository;
            this.baseService = baseService;
        }

        public IList<LateInEarlyOutViewModel> GetAllByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria)
        {
            IList<LateInEarlyOutViewModel> lateInEarlyOuts = baseService.GetData<LateInEarlyOutViewModel>(criteria);
            return lateInEarlyOuts;
        }

        public LateInEarlyOutViewModel GetById(GetLateInEarlyOutByIdCriteria criteria)
        {
            LateInEarlyOutViewModel lateInEarlyOut = baseService.GetDataEntity<LateInEarlyOutViewModel>(criteria);
            return lateInEarlyOut;
        }

        public IList<LateInEarlyOutViewModel> GetAllEmployee(GetAllEmployeeCriteria criteria)
        {
            IList<LateInEarlyOutViewModel> employeeList = baseService.GetData<LateInEarlyOutViewModel>(criteria);
            return employeeList;
        }

        public async Task<IEnumerable<LateInEarlyOutViolateModel>> GetLateInEarlyOutViolateOnMonth(GetLateInEarlyOutViolaterOnCurrentMonthCriteria criteria)
        {
            return await baseService.GetDataAsync<LateInEarlyOutViolateModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<LateInEarlyOutDiscipline>> GetLateInEarlyOutDiscipline(GetViolateLateInEarlyOutDisciplineCriteria criteria)
        {
            return await lateInEarlyOutrepository.GetLateInEarlyOutDiscipline(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<LateInEarlyOutModel>> GetLateInEarlyOutDeduction(GetLateInEarlyOutDeductionCriteria criteria)
        {
            return await baseService.GetDataAsync<LateInEarlyOutModel>(criteria).ConfigureAwait(false);
        }
    }
}