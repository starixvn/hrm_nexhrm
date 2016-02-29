namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Overtime;
    using Models;
    using ViewModels;

    public interface IOvertimeService
    {
        IList<Overtime> GetOvertime(GetOvertimeCriteria criteria);

        Overtime GetOvertimeById(GetOvertimeByIdCriteria criteria);

        IList<EmployeeOvertime> GetEmployeeOvertime(GetEmployeeOvertimeCriteria criteria);

        IList<EmployeeOvertime> GetMyOvertime(GetMyOvertimeCriteria criteria);
    }
}
