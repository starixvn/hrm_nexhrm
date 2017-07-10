namespace Fanex.NexHR.Services
{
    using Data.Repository;
    using Interfaces;
    using Models;
    using Repositories;
    using Repositories.Interfaces;
    using Resources;

    public class CommandService : ICommandService
    {
        private readonly IBaseRepository _repository;

        public CommandService()
            : this(null)
        {
        }

        public CommandService(IBaseRepository repository)
        {
            _repository = repository ?? new BaseRepository();
        }

        public CommandResult Insert(ICriteria criteria, bool hasGetOutputParams = false)
        {
            var result = criteria != null && criteria.IsValid()
                ? _repository.Insert(criteria, hasGetOutputParams)
                : GetInvalidInput();

            return result;
        }

        public CommandResult Update(ICriteria criteria, bool hasGetOutputParams = false)
        {
            var result = criteria != null && criteria.IsValid()
                ? _repository.Update(criteria, hasGetOutputParams)
                : GetInvalidInput();

            return result;
        }

        public CommandResult Delete(ICriteria criteria, bool hasGetOutputParams = false)
        {
            var result = criteria != null && criteria.IsValid()
                ? _repository.Delete(criteria, hasGetOutputParams)
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
