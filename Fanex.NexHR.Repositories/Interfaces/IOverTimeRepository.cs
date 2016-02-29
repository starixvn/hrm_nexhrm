namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Models;

    public interface IOvertimeRepository
    {
        IList<Overtime> GetOvertime(ICriteria criteria);

        Overtime GetOvertimeById(ICriteria criteria);

        IList<EmployeeOvertime> GetEmployeeOvertime(ICriteria criteria);
    }
}
