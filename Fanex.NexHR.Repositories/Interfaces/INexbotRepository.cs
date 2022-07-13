namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Repository;
    using Models.SkypeAccount;

    public interface INexbotRepository
    {
        Task<IEnumerable<EmployeeSkypeAccount>> GetEmployeeSkypeAccount(ICriteria criteria);
    }
}