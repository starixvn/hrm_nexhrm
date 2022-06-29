namespace Fanex.NexHR.Services.Interfaces
{
    using Data.Repository;
    using Models;

    public interface ICommandService
    {
        CommandResult ExecuteNonQuery(ICriteria criteria);
    }
}