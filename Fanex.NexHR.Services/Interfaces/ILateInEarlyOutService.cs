namespace Fanex.NexHR.Services.Interfaces
{
    using Criteria.LateInEarlyOut;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ViewModels.LateInEarlyOut;

    public interface ILateInEarlyOutService
    {
        Task<IEnumerable<LateInEarlyOutViewModel>> GetAllByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria);

        Task<LateInEarlyOutViewModel> GetById(GetLateInEarlyOutByIdCriteria criteria);

        Task<IEnumerable<LateInEarlyOutViewModel>> GetAllEmployee(GetAllLatInEarlyOutNotificationCriteria criteria);

        Task<IEnumerable<LateInEarlyOutViolateModel>> GetLateInEarlyOutViolateOnMonth(GetLateInEarlyOutViolaterOnCurrentMonthCriteria criteria);

        Task<IEnumerable<LateInEarlyOutDiscipline>> GetLateInEarlyOutDiscipline(GetViolateLateInEarlyOutDisciplineCriteria criteria);

        Task<IEnumerable<LateInEarlyOutModel>> GetLateInEarlyOutDeduction(GetLateInEarlyOutDeductionCriteria criteria);
        Task<IEnumerable<ReminderLateInEarlyOut>> GetAllReminderLateInEarlyOut(GetAllReminderLateInEarlyOut criteria);
    }
}