namespace Fanex.NexHR.Services
{
    using Data.Repository;
    using Interfaces;
    using Models;
    using Repositories.Interfaces;
    using Resources;

    public class CommandService : ICommandService
    {
        private readonly IBaseRepository repository;

        public CommandService(IBaseRepository repository)
        {
            this.repository = repository;
        }

        public CommandResult ExecuteNonQuery(ICriteria criteria)
        {
            var result = criteria != null && criteria.IsValid()
                ? repository.ExecuteNonQuery(criteria)
                : GetInvalidInput();

            return result;
        }

        private static CommandResult GetInvalidInput()
        {
            return new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };
        }
    }
}
