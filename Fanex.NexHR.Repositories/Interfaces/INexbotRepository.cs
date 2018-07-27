namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Data.Repository;
    using Models.SkypeAccount;

    public interface INexbotRepository
    {
        IEnumerable<EmployeeSkypeAccount> GetEmployeeSkypeAccount(ICriteria criteria);
    }
}