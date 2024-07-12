namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Repository;
    using Models.Overtime;

    public interface IOvertimeRepository
    {
        Task<IList<Overtime>> GetOvertime(ICriteria criteria);

        Task<Overtime> GetOvertimeById(ICriteria criteria);

        Task<IList<OvertimeSummary>> GetOvertimeSummary(ICriteria criteria);

        Task<IList<OvertimeConversion>> GetConversions(ICriteria criteria);
    }
}
