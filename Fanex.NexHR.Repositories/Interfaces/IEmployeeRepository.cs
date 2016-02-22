namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IEmployeeRepository
    {
        Employee GetById(ICriteria criteria);

        Employee GetByDomainName(ICriteria criteria);

        IList<Employee> Query(ICriteria criteria);

        MySpaceViewModel GetMySpace(ICriteria criteria);

        int GetRangeDate(ICriteria criteria);

        IList<Employee> GetEmployeeToeicScore(ICriteria criteria);

        SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria);
    }
}