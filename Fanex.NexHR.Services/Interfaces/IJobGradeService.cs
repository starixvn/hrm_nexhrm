namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.JobGrade;
    using Fanex.NexHR.Models;

    public interface IJobGradeService
    {
        Task<IEnumerable<EmployeeJobGrade>> GetEmployeeJobGrade(GetEmployeeJobGradeCriteria criteria);
    }
}
