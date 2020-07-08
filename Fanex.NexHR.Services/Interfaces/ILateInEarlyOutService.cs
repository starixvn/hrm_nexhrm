namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.LateInEarlyOut;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using ViewModels.LateInEarlyOut;

    public interface ILateInEarlyOutService
    {
        IList<LateInEarlyOutViewModel> GetAllByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria);

        LateInEarlyOutViewModel GetById(GetLateInEarlyOutByIdCriteria criteria);

        IList<LateInEarlyOutViewModel> GetAllEmployee(GetAllEmployeeCriteria criteria);

        Task<IEnumerable<LateInEarlyOutViolateModel>> GetLateInEarlyOutViolateOnMonth(GetLateInEarlyOutViolaterOnCurrentMonthCriteria criteria);
    }
}