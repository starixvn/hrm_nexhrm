namespace Fanex.NexHR.Services.Interfaces
{
    using Data.Repository;
    using Models;

    public interface ICommandService
    {
        CommandResult Insert(ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Update(ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Delete(ICriteria criteria, bool hasGetOutputParams = false);
    }
}