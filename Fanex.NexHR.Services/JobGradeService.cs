namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.JobGrade;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class JobGradeService : IJobGradeService
    {
        private readonly IBaseService baseService;

        public JobGradeService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<EmployeeJobGrade>> GetEmployeeJobGrade(GetEmployeeJobGradeCriteria criteria)
        {
            try
            {
                var employeeJobGrades = await baseService.GetDataAsync<EmployeeJobGrade>(criteria);
                return employeeJobGrades;
            }
            catch (Exception ex)
            {
                return new List<EmployeeJobGrade>();
            }
            
        }
    }
}
