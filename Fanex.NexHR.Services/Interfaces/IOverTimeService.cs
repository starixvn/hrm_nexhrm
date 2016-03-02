namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Overtime;
    using Models;
    using Models.Overtime;
    using System.Collections.Generic;

    public interface IOvertimeService
    {
        IList<Overtime> GetOvertime(GetOvertimeCriteria criteria);

        Overtime GetOvertimeById(GetOvertimeByIdCriteria criteria);

        IList<EmployeeOvertime> GetEmployeeOvertime(GetEmployeeOvertimeCriteria criteria);

        IList<EmployeeOvertime> GetMyOvertime(GetMyOvertimeCriteria criteria);
    }
}
