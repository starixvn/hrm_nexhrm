namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Threading.Tasks;
    using Data.Repository;
    using Models;

    public interface IBaseRepository
    {
        Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command);

        Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria);

        CommandResult ExecuteNonQuery(ICriteria criteria);
    }
}
