namespace Fanex.NexHR.Services
{
    using Criteria.LateInEarlyOut;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Fanex.NexHR.Repositories.Interfaces;
    using Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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

        public async Task<IEnumerable<LateInEarlyOutViewModel>> GetAllByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria)
        {
            IEnumerable<LateInEarlyOutViewModel> lateInEarlyOuts = await baseService.GetDataAsync<LateInEarlyOutViewModel>(criteria);
            return lateInEarlyOuts;
        }

        public async Task<LateInEarlyOutViewModel> GetById(GetLateInEarlyOutByIdCriteria criteria)
        {
            LateInEarlyOutViewModel lateInEarlyOut = await baseService.GetDataEntityAsync<LateInEarlyOutViewModel>(criteria);
            return lateInEarlyOut;
        }

        public async Task<IEnumerable<LateInEarlyOutViewModel>> GetAllEmployee(GetAllEmployeeCriteria criteria)
        {
            IEnumerable<LateInEarlyOutViewModel> employeeList = await baseService.GetDataAsync<LateInEarlyOutViewModel>(criteria);
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

        public async Task<IEnumerable<ReminderLateInEarlyOut>> GetAllReminderLateInEarlyOut(GetAllReminderLateInEarlyOut criteria)
        {
            return await baseService.GetDataAsync<ReminderLateInEarlyOut>(criteria).ConfigureAwait(false);
        }
    }
}