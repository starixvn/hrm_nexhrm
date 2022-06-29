namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.LateInEarlyOut;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Fanex.NexHR.Repositories.Interfaces;
    using Interfaces;
    using ViewModels.LateInEarlyOut;

    public class LateInEarlyOutService : BaseService, ILateInEarlyOutService
    {
        private readonly ILateInEarlyOutRepository lateInEarlyOutrepository;

        public LateInEarlyOutService(ILateInEarlyOutRepository lateInEarlyOutrepository)
        {
            this.lateInEarlyOutrepository = lateInEarlyOutrepository;
        }

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

        public async Task<IEnumerable<LateInEarlyOutViolateModel>> GetLateInEarlyOutViolateOnMonth(GetLateInEarlyOutViolaterOnCurrentMonthCriteria criteria)
        {
            return await GetDataAsync<LateInEarlyOutViolateModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<LateInEarlyOutDiscipline>> GetLateInEarlyOutDiscipline(GetViolateLateInEarlyOutDisciplineCriteria criteria)
        {
            return await lateInEarlyOutrepository.GetLateInEarlyOutDiscipline(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<LateInEarlyOutModel>> GetLateInEarlyOutDeduction(GetLateInEarlyOutDeductionCriteria criteria)
        {
            return await GetDataAsync<LateInEarlyOutModel>(criteria).ConfigureAwait(false);
        }
    }
}