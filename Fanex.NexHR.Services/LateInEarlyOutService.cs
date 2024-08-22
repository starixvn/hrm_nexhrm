namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria.LateInEarlyOut;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Interfaces;
    using ViewModels.LateInEarlyOut;

    public class LateInEarlyOutService : ILateInEarlyOutService
    {
        private readonly IBaseService baseService;

        public LateInEarlyOutService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<LateInEarlyOutViewModel>> GetAllByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria)
        {
            var lateInEarlyOuts = await baseService.GetDataAsync<LateInEarlyOutViewModel>(criteria);
            return lateInEarlyOuts;
        }

        public async Task<LateInEarlyOutViewModel> GetById(GetLateInEarlyOutByIdCriteria criteria)
        {
            LateInEarlyOutViewModel lateInEarlyOut = await baseService.GetDataEntityAsync<LateInEarlyOutViewModel>(criteria);
            return lateInEarlyOut;
        }

        public async Task<IEnumerable<LateInEarlyOutViewModel>> GetAllEmployee(GetAllLatInEarlyOutNotificationCriteria criteria)
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
            var result = await baseService.FetchMultipleAsync<LateInEarlyOutDiscipline, LateInEarlyOutDisciplineDetail>(criteria);
            var lateinEarlyOuts = result.Item1;
            var lateInEarlyOutDetails = result.Item2;

            foreach (var lateinEarlyOut in lateinEarlyOuts)
            {
                lateinEarlyOut.Details = lateInEarlyOutDetails.Where(e => e.EmpId == lateinEarlyOut.EmpId);
            }

            return lateinEarlyOuts;
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