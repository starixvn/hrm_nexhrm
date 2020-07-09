namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.LateInEarlyOut;

    public interface ILateInEarlyOutRepository
    {
        Task<IEnumerable<LateInEarlyOutDiscipline>> GetLateInEarlyOutDiscipline(ICriteria criteria);
    }
}