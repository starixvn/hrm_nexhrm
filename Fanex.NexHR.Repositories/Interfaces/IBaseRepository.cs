namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Threading.Tasks;
    using Data.Repository;
    using Models;

    public interface IBaseRepository
    {
        CommandResult Insert(ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Update(ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Delete(ICriteria criteria, bool hasGetOutputParams = false);

        Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria, bool hasGetOutputParams = false);
    }
}
