namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Models;
    using Models.Overtime;
    using System.Collections.Generic;

    public interface IOvertimeRepository
    {
        IList<Overtime> GetOvertime(ICriteria criteria);

        Overtime GetOvertimeById(ICriteria criteria);

        IList<EmployeeOvertime> GetEmployeeOvertime(ICriteria criteria);
    }
}
