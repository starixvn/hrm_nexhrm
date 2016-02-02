namespace Fanex.NexHR.Services
{
    using Criteria.Overtime;
    using Interfaces;
    using Models;
    using Repositories;
    using Repositories.Interfaces;
    using System.Collections.Generic;

    public class OvertimeService : BaseService, IOvertimeService
    {
        private readonly IOvertimeRepository _repository;

        public OvertimeService()
            : this(null)
        {
        }

        public OvertimeService(IOvertimeRepository repository)
        {
            _repository = repository ?? new OvertimeRepository();
        }

        public CommandResult Insert(InsertOvertimeCriteria criteria)
        {
            var result = Repository.Insert(criteria);

            return result;
        }

        public IEnumerable<Overtime> GetSummaryReport(GetSummaryCriteria criteria)
        {
            var result = _repository.GetSummaryReport(criteria);

            return result;
        }

        public CommandResult ApproveRequest(ApproveOvertimeRequestCriteria criteria)
        {
            return Repository.Update(criteria);
        }

        public CommandResult DisapproveRequest(DisapproveOvertimeRequestCriteria criteria)
        {
            return Repository.Update(criteria);
        }

        public CommandResult RejectRequest(UpdateStatusCriteria criteria)
        {
            return Repository.Update(criteria);
        }

        public Overtime Get(GetByIdCriteria criteria)
        {
            var model = _repository.Get(criteria);

            return model;
        }

        public CommandResult Update(UpdateOvertimeCriteria criteria)
        {
            return Repository.Update(criteria);
        }
    }
}
